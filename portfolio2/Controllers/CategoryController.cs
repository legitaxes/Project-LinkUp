﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using portfolio2.Models;
using portfolio2.DAL;

namespace portfolio2.Controllers
{
    public class CategoryController : Controller
    {
        private CategoryDAL categoryContext = new CategoryDAL();
        private SessionDAL sessionContext = new SessionDAL();
        private StudentDAL studentContext = new StudentDAL();
        private LocationDAL locationContext = new LocationDAL();

        public IActionResult Index()
        {
            List<Category> categoryList = categoryContext.GetAllCategory();
            return View(categoryList);
        }

        public ActionResult Filtered(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }
            List<Session> sessionList = sessionContext.FilteredSession(id);
            List<SessionViewModel> sessionDetailsList = MapToSessionVM(sessionList);
            Category cat = categoryContext.GetCategoryName(id);
            if (cat == null)
            {
                return RedirectToAction("Index");
            }
            ViewData["CategoryName"] = cat.CategoryName;
            if (sessionDetailsList.Count == 0)
            {
                ViewData["Message"] = "It doesn't seem like there is any session under ";
            }
            return View(sessionDetailsList);
        }

        public ActionResult Search(string searchedvalue, string type)
        {
            if (type == "sessions")
            {
                //TempData["Searched"] = searchedvalue;
                //TempData["Type"] = type;
                return RedirectToAction("Search", "Session", new { searchedvalue = searchedvalue, type = type });
            }
            else if (type == "users")
            {
                //TempData["Search"] = searchedvalue;
                //TempData["Type"] = type;
                return RedirectToAction("Search", "Session", new { searchedvalue = searchedvalue, type = type });
            }
            return RedirectToAction("Error", "Home");
        }

        public List<SessionViewModel> MapToSessionVM(List<Session> sessionList)
        {
            string studentName = "";
            string locationName = "";
            string categoryName = "";
            List<StudentDetails> studentList = studentContext.GetAllStudent();
            List<Location> locationList = locationContext.GetAllLocations();
            List<Category> categoryList = categoryContext.GetAllCategory();
            List<SessionViewModel> sessionViewModelList = new List<SessionViewModel>();
            foreach (Session session in sessionList)
            {
                foreach (StudentDetails student in studentList)
                {
                    if (session.StudentID == student.StudentID)
                    {
                        studentName = student.Name;
                        break;
                    }
                }
                foreach (Location location in locationList)
                {
                    if (session.LocationID == location.LocationID)
                    {
                        locationName = location.LocationName;
                        break;
                    }
                }
                foreach (Category category in categoryList)
                {
                    if (category.CategoryID == session.CategoryID)
                    {
                        categoryName = category.CategoryName;
                    }
                }
                sessionViewModelList.Add(
                    new SessionViewModel
                    {
                        SessionID = session.SessionID,
                        SessionDate = session.SessionDate,
                        Name = session.Name,
                        Description = session.Description,
                        Photo = session.Photo,
                        Hours = session.Hours,
                        Participants = session.Participants,
                        Status = session.Status,
                        StudentName = studentName,
                        LocationName = locationName,
                        CategoryName = categoryName
                    });
            }
            return sessionViewModelList;
        }

    }
}