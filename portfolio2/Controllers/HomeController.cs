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

        //login action method for homepage
        [HttpPost]
        public ActionResult Login(IFormCollection formData)
        {
            // Read inputs from textboxes
            // Email address converted to lowercase
            string loginID = formData["txtLoginID"].ToString().ToLower();
            string password = formData["txtPassword"].ToString();
            if (loginID == "abc@npbook.com" && password == "pass1234")
            {
                // Redirect user to the "StaffMain" view through an action
                return RedirectToAction("VolunteerMain");
            }
            else
            {
                // Redirect user back to the index view through an action
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult VolunteerMain()
        {
            return View();
        }

    }
}