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
        [Authorize]
        public async Task<ActionResult> StudentLogin()
        {
            //Retrieve the access token of the user
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            //Call API to obtain user information
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://ictonejourney.com");
            client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await
             client.GetAsync("/api/Users/userinfo");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                //Convert the JSON string into an Account object
                Account account = JsonConvert.DeserializeObject<Account>(data);
                HttpContext.Session.SetString("LoginID", account.Accounts.Name);
                HttpContext.Session.SetString("Role", "Student");
                HttpContext.Session.SetString("LoggedInTime",
                 DateTime.Now.ToString());
                return RedirectToAction("Index", "Book");
            }
            return RedirectToAction("Index");
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