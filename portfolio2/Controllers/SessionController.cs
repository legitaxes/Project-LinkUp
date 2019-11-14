using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using portfolio2.DAL;
using portfolio2.Models;

namespace portfolio2.Controllers
{
    public class SessionController : Controller
    {
        private SessionDAL sessionContext = new SessionDAL();
        private StudentDAL studentContext = new StudentDAL();
        private LocationDAL locationContext = new LocationDAL();
        private CategoryDAL categoryContext = new CategoryDAL();

        public IActionResult Index()
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
                        Status = session.Status,
                        StudentName = studentName,
                        LocationName = locationName,
                        CategoryName = categoryName
                    });
            }
            return sessionViewModelList;
        }
        public SessionPhoto MapToSingleSessionVM(Session session)
        {
            string studentName = "";
            string locationName = "";
            string categoryName = "";
            List<StudentDetails> studentList = studentContext.GetAllStudent();
            List<Location> locationList = locationContext.GetAllLocations();
            List<Category> categoryList = categoryContext.GetAllCategory();
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
            SessionPhoto newSession = new SessionPhoto();
            newSession.SessionID = session.SessionID;
            newSession.SessionDate = session.SessionDate;
            newSession.Name = session.Name;
            newSession.Description = session.Description;
            newSession.Photo = session.Photo;
            newSession.Hours = session.Hours;
            newSession.Participants = session.Participants;
            newSession.Status = session.Status;
            newSession.StudentName = studentName;
            newSession.LocationName = locationName;
            newSession.CategoryName = categoryName;
            return newSession;
        }

        public ActionResult Create()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["CategoryList"] = categoryContext.GetCategoryList();
            ViewData["LocationList"] = locationContext.GetLocationList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Session session)
        {
            session.StudentID = HttpContext.Session.GetInt32("StudentID");
            session.Photo = "stocksession.jpg";
            ViewData["CategoryList"] = categoryContext.GetCategoryList();
            ViewData["LocationList"] = locationContext.GetLocationList();
            session.Participants = 0;
            System.Diagnostics.Debug.WriteLine(session);
            session.Status = 'N';
            if (ModelState.IsValid)
            {
                session.SessionID = sessionContext.CreateSession(session);
                ViewData["Message"] = "Session Posted Successfully";
                session.DateCreated = DateTime.Now;
                TempData["Session"] = session;
                return RedirectToAction("UploadSessionPhoto");
            }
            else
            {
                ViewData["Message"] = "There is something wrong. Please Try Again!";
                return View(session);
            }
        }
        public ActionResult MySession()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<Session> sessionList = sessionContext.GetMySession(HttpContext.Session.GetInt32("StudentID"));
            List<SessionViewModel> sessionDetailsList = MapToSessionVM(sessionList);
            return View(sessionDetailsList);
        }

        public ActionResult UploadSessionPhoto()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Index", "Home");
            }
            Session session = TempData["Session"] as Session;
            SessionPhoto newSession = MapToSingleSessionVM(session);
            return View(session);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSessionPhoto(SessionPhoto session)
        {
            if (session.FileToUpload != null && session.FileToUpload.Length > 0)
            {
                try
                { // Find the filename extension of the file to be uploaded.
                    string fileExt = Path.GetExtension(session.FileToUpload.FileName);
                    string uploadedFile = session.SessionID + fileExt;
                    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\sessions", uploadedFile);
                    using (var fileSteam = new FileStream(savePath, FileMode.Create))
                    {
                        await session.FileToUpload.CopyToAsync(fileSteam);
                    }
                    session.Photo = uploadedFile;
                    ViewData["Message"] = "File uploaded successfully.";
                }
                catch (IOException)
                {
                    ViewData["Message"] = "File uploading fail!";
                }
                catch (Exception ex)
                {
                    ViewData["Message"] = ex.Message;
                }
            }
            return View(session);
        }
    }
}