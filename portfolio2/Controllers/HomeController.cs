using System;
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
    public class HomeController : Controller
    {
        private StudentDAL studentContext = new StudentDAL();
        private SessionDAL sessionContext = new SessionDAL();
        private LocationDAL locationContext = new LocationDAL();
        private CategoryDAL categoryContext = new CategoryDAL();

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
                return RedirectToAction("StudentMain");
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
                HttpContext.Session.SetString("LoginID", account.Student.Name);
                HttpContext.Session.SetString("StudentID", account.Student.EmailId);
                HttpContext.Session.SetString("Role", "Student");
                HttpContext.Session.SetString("LoggedInTime",
                 DateTime.Now.ToString());
                return RedirectToAction("Index", "Student");
            }
            return RedirectToAction("Index");
        }

        public ActionResult StudentMain()
        {
            //if student never signup on our site before
            if (studentContext.checkStudent(HttpContext.Session.GetString("StudentID")) == false)
                return RedirectToAction("Create", "Student");
            else
                return View();
        }

        public ActionResult ViewSession()
        {
            List<Session> sessionList = sessionContext.GetAllSessions();
            List<SessionViewModel> sessionDetailsList = MapToSessionVM(sessionList);
            return View(sessionDetailsList);
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
                        StudentName = studentName,
                        LocationName = locationName,
                        CategoryName = categoryName
                    });
            }
            return sessionViewModelList;
        }

        //logout button function
        public ActionResult LogOut()
        { 
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}