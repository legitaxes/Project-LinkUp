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
        private NotificationDAL notificationContext = new NotificationDAL();

        public IActionResult Index()
        {
            List<Category> categoryList = categoryContext.GetAllCategory();
            return View(categoryList);
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
                HttpContext.Session.SetString("StudentNumber", account.Student.EmailId);
                StudentDetails student = studentContext.GetStudentDetails(HttpContext.Session.GetString("StudentNumber"));
                if (student != null)
                {
                    HttpContext.Session.SetInt32("StudentID", student.StudentID);
                    if (student.Photo != null)
                        HttpContext.Session.SetString("Photo", student.Photo);
                }
                HttpContext.Session.SetString("Role", "Student");
                HttpContext.Session.SetString("LoggedInTime",
                 DateTime.Now.ToString());
                if (studentContext.checkStudent(HttpContext.Session.GetString("StudentNumber")) == false)
                    return RedirectToAction("Create", "Student");
                return RedirectToAction("StudentMain");
            }
            return RedirectToAction("Index");
        }

        public ActionResult StudentMain()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            //if student never signup on our site before
            if (studentContext.checkStudent(HttpContext.Session.GetString("StudentNumber")) == false)
                return RedirectToAction("Create", "Student");
            else
            {
                List<Notification> notificationList = notificationContext.GetAllNotification(HttpContext.Session.GetInt32("StudentID"));
                return View(notificationList);
            }
        }

        public ActionResult GiveSessionOwnerReview(int id, int sessionid, int notificationid)
        {
            TempData["SessionStatus"] = 'Y'; //to get past the prevention system in check
            TempData["SessionID"] = sessionid; //sets the sessionID for the notification
            TempData["StudentReview"] = true;
            TempData["NotificationID"] = notificationid;
            return RedirectToAction("StudentReview", "Session", new { id = id });
        }

        public ActionResult Error() //main error page with fake error 404 message
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