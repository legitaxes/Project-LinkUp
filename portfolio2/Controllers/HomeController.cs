using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace portfolio2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //login action method for homepage --- to be worked on --- need to include hashing and implementation of db
        [HttpPost]
        public ActionResult Login(IFormCollection formData)
        {
            // Read inputs from textboxes
            // Email address converted to lowercase
            string loginID = formData["txtLoginID"].ToString().ToLower();
            string password = formData["txtPassword"].ToString();
            if (loginID == "abc@npbook.com" && password == "pass1234")
            {
                HttpContext.Session.SetString("Role", "Student");
                HttpContext.Session.SetString("LoginName", loginID);
                //HttpContext.Session.SetInt32("ID", lecturer.LecturerId.ToString());  //remember to add
                return RedirectToAction("VolunteerMain");
            }
            else
            {
                TempData["Message"] = "Invalid Login Credentials!";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult VolunteerMain()
        {
            return View();
        }

        //logout button function
        public ActionResult LogOut()
        { 
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}