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
            }            List<Student> studentList = studentContext.GetAllStudent();
            return View(studentList);
        }
    }
}