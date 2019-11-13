﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using portfolio2.DAL;
using portfolio2.Models;
using System.IO;

namespace portfolio2.Controllers
{
    public class StudentController : Controller
    {
        private StudentDAL studentContext = new StudentDAL();
        private CourseDAL courseContext = new CourseDAL();
        private StudentRatingDAL studentratingContext = new StudentRatingDAL();
        private RatingDAL ratingContext = new RatingDAL();

        // GET: Student Method
        public IActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
             (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Index", "Home");
            }

            //if student never signup on our site before
            if (studentContext.checkStudent(HttpContext.Session.GetString("StudentNumber")) == false)
            {
                return RedirectToAction("Create");
            }
            
            //List<StudentDetails> studentList = studentContext.GetAllStudent();
            return View();
        }

        private List<SelectListItem> DropDownCourse()
        {
            List<SelectListItem> course = new List<SelectListItem>();
            List<Course> allcourselist = courseContext.getAllCourse();
            course.Add(new SelectListItem
            {
                Value = "",
                Text = "---Select Course---"
            });
            foreach (Course availablecourse in allcourselist)
            {
                course.Add(new SelectListItem
                {
                    Value = availablecourse.CourseID.ToString(),
                    Text = availablecourse.CourseName
                });
            }        
            return course;
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
            student.StudentNumber = HttpContext.Session.GetString("StudentNumber");
            student.Name = HttpContext.Session.GetString("LoginID");
            student.PhoneNo = null;
            ViewData["Courselist"] = DropDownCourse();
            //int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            student.Year = null;
            return View(student);
        }

        [HttpPost]
        public ActionResult Create(StudentDetails student)
        {
            student.Points = null;
            student.Photo = null;
            student.Name = HttpContext.Session.GetString("LoginID");
            student.StudentNumber = HttpContext.Session.GetString("StudentNumber");
            if (ModelState.IsValid)
            {
                ViewData["Message"] = "Student Profile Updated Successfully";
                student.StudentID = studentContext.Add(student);
                HttpContext.Session.SetInt32("StudentID", student.StudentID);
                ViewData["Courselist"] = DropDownCourse();
                return View(student);
            }
            ViewData["Message"] = "Something went wrong! Please try again!";
            ViewData["Courselist"] = DropDownCourse();
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

            StudentDetails student = studentContext.GetStudentDetails(HttpContext.Session.GetString("StudentNumber"));
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
            int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            StudentViewModel studentList = new StudentViewModel();
            List<StudentDetails> allstudentList = studentContext.GetAllStudent();
            foreach (StudentDetails student in allstudentList)
            {
                if (student.StudentID == studentid)
                {
                    StudentViewModel studentVM = MapToMultipleVMs(student);
                    studentList = studentVM;
                }
       
            }
            // StudentViewModel studentVM = MapToStudentVM(student); //to be completed - 1. student details 2. student course 3. student skillset 4. student rating
            return View(studentList);
        }

        public ActionResult Photo()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
           (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Index", "Home");
            }
            int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            StudentPhoto studentPhoto = studentContext.GetPhotoDetails(studentid);
            return View(studentPhoto);
        }

        //Button on updating student profile photo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Photo(StudentPhoto student)
        {
            if (student.FileToUpload != null && student.FileToUpload.Length > 0)
            {
                try
                {
                    // Find the filename extension of the file to be uploaded.
                    string fileExt = Path.GetExtension(student.FileToUpload.FileName);
                    // Rename the uploaded file with the staff’s name.
                    string uploadedFile = student.Name + fileExt;
                    // Get the complete path to the images folder in server
                    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Profilepictures", uploadedFile);
                    // Upload the file to server
                    using (var fileSteam = new FileStream(savePath, FileMode.Create))
                    {
                        await student.FileToUpload.CopyToAsync(fileSteam);
                    }
                    student.Photo = uploadedFile;
                    studentContext.UploadPhoto(student);
                    ViewData["Message"] = "File uploaded successfully.";
                }
                catch (IOException)
                {
                    //File IO error, could be due to access rights denied 
                    ViewData["Message"] = "File uploading fail!";
                }
                catch (Exception ex)
                //Other type of error 
                {
                    ViewData["Message"] = ex.Message;
                }
            }
            return View(student);
        }

        public StudentViewModel MapToMultipleVMs(StudentDetails student)
        {
            string coursename = "";
            int totalrating = 0;
            int amountofratings = 0;
            int averagerating = 0;

            List<Course> courseList = courseContext.getAllCourse();
            foreach (Course currentcourse in courseList)
            {
                if (currentcourse.CourseID == student.CourseID)
                {
                    coursename = currentcourse.CourseName;
                }
            }

            List<StudentRating> studentratingList = studentratingContext.GetAllStudentRatings();
            List<Rating> ratingList = ratingContext.GetAllRatings();
            foreach (StudentRating currentstudentrating in studentratingList)
            {
                if (currentstudentrating.StudentID == student.StudentID)
                {
                    foreach (Rating currentrating in ratingList)
                    {
                        if (currentrating.RatingID == currentstudentrating.RatingID)
                        {
                            totalrating = totalrating + currentrating.Stars;
                            amountofratings++;
                        }
                    }
                }
            }

            if (amountofratings > 0)
            {
                averagerating = totalrating / amountofratings;
            }

            else
            {
                averagerating = 0;
            }

            StudentViewModel studentVM = new StudentViewModel
            {
                StudentID = student.StudentID,
                Name = student.Name,
                Year = student.Year,
                StudentNumber = student.StudentNumber,
                Photo = student.Photo,
                PhoneNo = student.PhoneNo,
                Interest = student.Interest,
                ExternalLink = student.ExternalLink,
                Description = student.Description,
                Points = student.Points,
                CourseID = student.CourseID,
                CourseName = coursename,               
                Rating = averagerating,
                TotalRatings = amountofratings
            };
            return studentVM;
        }      
    }
}