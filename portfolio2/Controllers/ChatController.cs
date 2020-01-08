using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace portfolio2.Controllers
{
    public class ChatController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("InsertUserName");
        }

        [HttpPost]
        public IActionResult Index(string username)
        {
            return View("Index", username);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}