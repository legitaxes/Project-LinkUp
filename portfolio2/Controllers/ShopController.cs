using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using portfolio2.DAL;
using portfolio2.Models;

namespace portfolio2.Controllers
{
    public class ShopController : Controller
    {
        private ShopDAL shopContext = new ShopDAL();
        private StudentDAL studentContext = new StudentDAL();

        public IActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            List<Shop> allstoreList = shopContext.GetAllStoreItems();
            ViewBag.List = allstoreList;
            int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            StudentDetails student = studentContext.GetStudentDetails(studentid);
            ViewData["StudentPoints"] = student.Points;
            return View();
        }

        public IActionResult Purchase(int? id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }
            Shop item = shopContext.GetItemByID(id);
            if (item == null)
            {
                return RedirectToAction("Error", "Home");
            }
            int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            StudentDetails student = studentContext.GetStudentDetails(studentid);
            int studentpoints = Convert.ToInt32(student.Points);
            if (studentpoints < item.Cost)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(item);
        }

        [HttpPost]
        public IActionResult Purchase(Shop item)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }

            int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            StudentDetails student = studentContext.GetStudentDetails(studentid);
            int studentpoints = Convert.ToInt32(student.Points);
            if (studentpoints < item.Cost)
            {
                return RedirectToAction("Error", "Home");
            }

            shopContext.Purchase(item.Cost, studentid, studentpoints);
            return RedirectToAction("Index", "Shop");
        }
    }
}