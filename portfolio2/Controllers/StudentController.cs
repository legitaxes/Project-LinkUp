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
            }            //if student never signup on our site before            if (studentContext.checkStudent(HttpContext.Session.GetString("LoginID")))
            {
                return RedirectToAction("Create");
            }                        List<Student> studentList = studentContext.GetAllStudent();
            return View(studentList);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(Student student)
        {
            student.Points = null;
            student.Photo = null;
            if (ModelState.IsValid)
            {
                ViewData["Message"] = "Student Created Successfully";
                student.StudentID = studentContext.Add(student);
            }
            return null;
        }
    }
}