using System;
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

        // GET: Student Method
        public IActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
             (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Index", "Home");
            }            //if student never signup on our site before            if (studentContext.checkStudent(HttpContext.Session.GetString("StudentID")) == false)
            {
                return RedirectToAction("Create");
            }                        //List<StudentDetails> studentList = studentContext.GetAllStudent();
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
    }
}