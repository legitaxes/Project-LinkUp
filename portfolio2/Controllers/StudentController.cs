﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using portfolio2.DAL;
using portfolio2.Models;

namespace portfolio2.Controllers
{
    public class StudentController : Controller
    {
        private StudentDAL studentContext = new StudentDAL();
        private CourseDAL courseContext = new CourseDAL();
        
        // GET: Student Method
        public IActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
             (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Index", "Home");
            }

            //if student never signup on our site before
            if (studentContext.checkStudent(HttpContext.Session.GetString("StudentID")) == false)
            {
                return RedirectToAction("Create");
            }
            
            //List<StudentDetails> studentList = studentContext.GetAllStudent();
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Index", "Home");
            }
            StudentDetails student = new StudentDetails();
            student.StudentNumber = HttpContext.Session.GetString("StudentID");
            student.Name = HttpContext.Session.GetString("LoginID");
            student.PhoneNo = null;
            student.Year = null;
            return View(student);
        }

        [HttpPost]
        public ActionResult Create(StudentDetails student)
        {
            student.Points = null;
            student.Photo = null;
            student.Name = HttpContext.Session.GetString("LoginID");
            student.StudentNumber = HttpContext.Session.GetString("StudentID");

            if (ModelState.IsValid)
            {
                ViewData["Message"] = "Student Profile Updated Successfully";
                student.StudentID = studentContext.Add(student);
                return View(student);
            }
            ViewData["Message"] = "Something went wrong! Please try again!";
            return View(student);
        }

        [HttpGet]
        public ActionResult Update()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
               (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Index", "Home");
            }

            StudentDetails student = studentContext.GetStudentDetails(HttpContext.Session.GetString("StudentID"));
            return View(student);
        }
        [HttpPost]
        public ActionResult Update(StudentDetails student)
        {
            if (ModelState.IsValid)
            {
                studentContext.Update(student);
                ViewData["Message"] = "Profile updated successfully!";
                return View(student);
            }
            ViewData["Message"] = "Could Not Update Profile. Please Try Again!";
            return View(student);
        }

        //view the details of the logged in user
        public ActionResult Details()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
               (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Index", "Home");
            }
            StudentDetails student = studentContext.GetStudentDetails(HttpContext.Session.GetString("StudentID"));
            StudentViewModel studentVM = MapToStudentVM(student); //to be completed - 1. student details 2. student course 3. student skillset 4. student rating
            return View(student);
        }

        //to be completed - 
        //1. student details
        //2. student course 
        //3. student skillset 
        //4. student rating
        public StudentViewModel MapToStudentVM(StudentDetails student)
        {
            string courseName = "";
            int ratingcount;
            List<Course> courseList = courseContext.getAllCourse();
            foreach (Course course in courseList)
            {
                if (course.CourseID == student.CourseID)
                {
                    courseName = course.CourseName;
                    break;
                }
            }
            StudentViewModel studentVM = new StudentViewModel
            {
                StudentID = student.StudentID,
                Name = student.Name,
                Year = student.Year,
                StudentNumber = student.StudentNumber,
                Photo = student.Name + ".jpg",
                PhoneNo = student.PhoneNo,
                ExternalLink = student.ExternalLink,
                Description = student.Description,
                Points = student.Points,
                CourseName = courseName
                //to be done
            };

            return studentVM;
        }
    }
}