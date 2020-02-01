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
    public class SessionController : Controller
    {
        private SessionDAL sessionContext = new SessionDAL();
        private StudentDAL studentContext = new StudentDAL();
        private LocationDAL locationContext = new LocationDAL();
        private CategoryDAL categoryContext = new CategoryDAL();
        private RatingDAL ratingContext = new RatingDAL();
        private NotificationDAL notificationContext = new NotificationDAL();

        public IActionResult Index()
        {
            List<Session> sessionList = sessionContext.GetAllSessions();
            List<SessionViewModel> sessionDetailsList = MapToSessionVM(sessionList);
            return View(sessionDetailsList);
        }

        public List<SessionViewModel> MapToSessionVM(List<Session> sessionList) //maps a list of sessions
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
                        Points = session.Points,
                        Status = session.Status,
                        StudentName = studentName,
                        LocationName = locationName,
                        CategoryName = categoryName
                    });
            }
            return sessionViewModelList;
        }

        public ActionResult ViewSessionParticipant(int? id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            bool owner = sessionContext.CheckSessionOwner(id, HttpContext.Session.GetInt32("StudentID"));
            if (owner == false)
            {
                return RedirectToAction("Error", "Home");
            }
            List<StudentDetails> participantList = sessionContext.GetParticipantList(id);
            Session currentSession = sessionContext.GetSessionDetails(id);
            ViewData["SessionName"] = currentSession.Name;
            return View(participantList);
        }

        public ActionResult Search(string searchedvalue, string type)
        {
            if (type == "sessions")
            {
                List<Session> sessionList = sessionContext.GetSearchedSession(searchedvalue);
                List<SessionViewModel> sessionViewModelList = MapToSessionVM(sessionList);
                ViewData["Searched"] = searchedvalue;
                return View(sessionViewModelList);
            }
            else if (type == "users")
            {
                ViewData["Search"] = searchedvalue;
                return RedirectToAction("StudentList", "Student", new { searchedvalue = searchedvalue });
            }
            return View();
        }

        public ActionResult Details(int id)
        {
            if (id == null) // if id is null
            {
                return RedirectToAction("Error", "Home");
            }
            Session session = sessionContext.GetSessionDetails(id);
            if (session == null) //if session is not found in the database
            {
                return RedirectToAction("Error", "Home");
            }
            SessionPhoto currentSession = MapToSingleSessionVM(session);
            bool owner = sessionContext.CheckSessionOwner(id, HttpContext.Session.GetInt32("StudentID")); //checks whether the session is the session owner
            if (owner == true) //checks whether the session is created by the user
            {
                ViewData["Owner"] = "This is the session owner";
            }
            else //else, - not owner.. check whether the user have already signup for the session
            {
                bool checksignup = sessionContext.CheckSignUp(id, HttpContext.Session.GetInt32("StudentID"));
                if (HttpContext.Session.GetInt32("StudentID") != null)
                {
                    int bookingid = sessionContext.GetBookingID(HttpContext.Session.GetInt32("StudentID"), id);
                    bool statuscancelled = sessionContext.CheckBookingStatus(bookingid, id);
                    if (checksignup == true && statuscancelled == false) //checks whether the user have already signed up for the session
                    {
                        ViewData["CheckSignUp"] = "The User have signed up";
                    }
                    else if (checksignup == true && statuscancelled == true)
                    {
                        ViewData["StatusCancelled"] = "The session was cancelled...";
                    }
                }
            }
            DateTime currenttime = DateTime.Now;
            TimeSpan ts = session.SessionDate - currenttime;
            if (ts.TotalHours < 2) //checks whether the session is 2 hours before || this is used for cancelling sign up (users) or cancelling session (session owner) 
            {
                ViewData["Message"] = "You are not allowed to cancel the session 2 hours before and during the session!";
            }
            var sesover = session.SessionDate + TimeSpan.FromHours(session.Hours); //adds number of hours of a session to sessiondate of session, this gives the time the session ends
            if ((sesover - DateTime.Now).TotalHours <= 0) //take sesover deduct datetime.now, it should give negative number: meaning it is way past the session time. 
            {                                             //if it is positive: the session is not over/not yet begun. ||(USE BREAKPOINTS HERE TO UNDERSTAND)||
                ViewData["SessionOver"] = "The session is over... Click to mark as complete";
            }
            StudentDetails sessionOwner = studentContext.GetStudentBasedOnSession(id); //this gets the student details of the session owner!
            if (sessionOwner == null)
                return RedirectToAction("Error", "Home");
            ViewData["SessionStudentNumber"] = sessionOwner.StudentNumber;
            if ((HttpContext.Session.GetString("Role") == null) || //checks whether the user is logged in
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                ViewData["Session"] = "This is not logged in";
            }
            return View(currentSession);
        }

        [HttpPost]
        public ActionResult Details(SessionViewModel session)
        {
            int bookingID = sessionContext.CreateBooking(session.SessionID, session.Hours); //this creates a booking in the Booking Table
            sessionContext.CreateStudentBooking(HttpContext.Session.GetInt32("StudentID"), bookingID); //this updates the StudentBooking Table
            sessionContext.UpdateSessionParticipant(session.Participants + 1, session.SessionID); //this increases participant count
            return RedirectToAction("SignUp");
        }

        [HttpPost]
        public ActionResult CancelSignUp(SessionViewModel session)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            int bookingid = sessionContext.GetBookingID(HttpContext.Session.GetInt32("StudentID"), session.SessionID);
            TempData["Cancel"] = "Sign up Cancelled Successfully";
            sessionContext.RemoveStudentBooking(HttpContext.Session.GetInt32("StudentID"), bookingid);
            sessionContext.RemoveBooking(bookingid);
            sessionContext.UpdateSessionParticipant(session.Participants - 1, session.SessionID);
            return RedirectToAction("Details", new { id = session.SessionID });
        }

        public ActionResult SignUp() //this page is used to show a message of the user successfully signing up
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            return View();
        }

        public ActionResult GiveStudentReview(int? id) //session id passed in
        {
            if (id == null) //return to error page if user tries enter the page without any id
            {
                return RedirectToAction("Error", "Home");
            }
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            Session session = sessionContext.GetSessionDetails(id); //get session details
            if (session == null)
            {
                return RedirectToAction("Error", "Home");
            }
            if (session.Status == 'N')
            {
                return RedirectToAction("Error", "Home");
            }
            SessionPhoto currentSession = MapToSingleSessionVM(session); //convert to viewmodel
            bool owner = sessionContext.CheckSessionOwner(id, HttpContext.Session.GetInt32("StudentID")); //checks whether the session is the session owner || false = not owner
            if (owner == false) //checks whether the session is the session owner, ONLY SESSION OWNER CAN MARK THEIR OWN SESSION COMPLETE
            {                   // this check ensures that the user who tries to cheat his way through url manipulation will not even get pass here
                return RedirectToAction("Error", "Home");
            }
            TempData["SessionID"] = session.SessionID;
            TempData["SessionStatus"] = session.Status;
            List<StudentDetails> SessionParticipantList = sessionContext.GetParticipantList(id);
            return View(SessionParticipantList);
        }

        public ActionResult StudentReview(int id) // studentid to give review || this page is flawed... anyone can give reviews to anyone (that's if anyone bother reading through the code LUL
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            if (id == null) //return to error page if user tries enter the page without any id
            {
                return RedirectToAction("Error", "Home");
            }
            if (Convert.ToChar(TempData["SessionStatus"]) == 'N' || TempData["SessionStatus"] == null)
            {
                return RedirectToAction("Error", "Home");
            }
            StudentDetails student = studentContext.GetStudentDetails(id); //gets the student details and store them in a viewbag below
            if (student == null) //return to error page if the student cannot be found
            {
                return RedirectToAction("Error", "Home");
            }
            ViewData["Stars"] = GetRatingStar(); //sets stars
            TempData["id"] = id; //sets studentid to tempdata[id]
            ViewBag.StudentDetails = student;
            return View();
        }

        [HttpPost]
        public ActionResult StudentReview(Rating review) // studentid to give review || this page is flawed... anyone can give reviews to anyone (that's if anyone bother reading through the code LUL
        {
            ViewData["Stars"] = GetRatingStar();
            if (ModelState.IsValid)
            {
                int studentid = Convert.ToInt32(TempData["id"]);
                if (TempData["StudentReview"] != null)
                {
                    int ratingid = ratingContext.GiveReviewToSessionOwner(review); //updates the rating table with the new rating given by the user
                    ratingContext.UpdateStudentRating(studentid, ratingid); //updates studentrating table based on the ratings
                    notificationContext.AddReviewGivenNotification(studentid, HttpContext.Session.GetInt32("StudentID")); //gives a notification to the user if they have received a review
                    int bookingid = sessionContext.GetBookingID(studentid, review.SessionID); //gets bookingid based on studentid and sessionid
                    //sessionContext.RemoveStudentBooking(studentid, bookingid); //removes studentbooking based on the studentid and bookingid
                    sessionContext.UpdateBookingStatus(bookingid); //updates the bookingstatus for the student to be 'Y'
                    notificationContext.RemoveNotification(Convert.ToInt32(TempData["NotificationID"]));
                    return RedirectToAction("StudentMain", "Home");
                }
                else
                {
                    int ratingid = ratingContext.GiveReviewToParticipant(review); //updates the rating table with the new rating given by the user
                    ratingContext.UpdateStudentRating(studentid, ratingid); //updates studentrating table based on the ratings
                    notificationContext.AddReviewGivenNotification(studentid, HttpContext.Session.GetInt32("StudentID")); //gives a notification to the user if they have received a review
                    int bookingid = sessionContext.GetBookingID(studentid, review.SessionID); //gets bookingid based on studentid and sessionid
                    //sessionContext.RemoveStudentBooking(studentid, bookingid); //removes studentbooking based on the studentid and bookingid
                    sessionContext.UpdateBookingStatus(bookingid); //updates the bookingstatus for the student to be 'Y'
                    return RedirectToAction("GiveStudentReview", new { id = review.SessionID });
                }
            }
            ViewData["ErrorMessage"] = "You cannot submit an enter review! Please Try Again";
            return RedirectToAction("GiveStudentReview", new { id = review.SessionID });
        }

        private List<SelectListItem> GetRatingStar()
        {
            List<SelectListItem> stars = new List<SelectListItem>();
            for (int i = 1; i <= 5; i++)
            {
                stars.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = i.ToString(),
                });
            }
            return stars;
        }

        public ActionResult MarkSessionComplete(int? id) //change the status of the session to Y, update the points to the participants and the session owner + participants
        {
            if (id == null) //return to error page if user tries enter the page without any id
            {
                return RedirectToAction("Error", "Home");
            }
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            Session session = sessionContext.GetSessionDetails(id);
            if (session == null)
            {
                return RedirectToAction("Error", "Home");
            }
            bool owner = sessionContext.CheckSessionOwner(id, HttpContext.Session.GetInt32("StudentID")); //checks whether the session is the session owner || false = not owner
            if (owner == false) //checks whether the session is the session owner, ONLY SESSION OWNER CAN MARK THEIR OWN SESSION COMPLETE
            {                   // this check ensures that the user who tries to cheat his way through url manipulation will not even get pass here
                return RedirectToAction("Error", "Home");
            }
            SessionPhoto currentSession = MapToSingleSessionVM(session);
            int sessionPoints = currentSession.Points;
            StudentDetails sessionOwner = studentContext.GetStudentBasedOnSession(id); //gets session owner details
            sessionContext.MarkSessionAsComplete(id); //sets session table --> "Status" as 'Y'
            if (currentSession.Participants != 0) //only give points if there is at least 1 participant
            {
                studentContext.UpdateStudentPoints(sessionOwner.StudentID, sessionOwner.Points + sessionPoints);
                List<StudentDetails> participantList = sessionContext.GetParticipantList(id);
                if (participantList.Count > 0)
                {
                    foreach (StudentDetails participant in participantList)
                    {
                        //to implement: attendance checklist or remove a student who didnt turn up
                        studentContext.UpdateStudentPoints(participant.StudentID, participant.Points + (sessionPoints/2)); //distribute the points to the participants for participanting
                        notificationContext.AddReviewNotification(participant.StudentID, HttpContext.Session.GetInt32("StudentID"), id); //gives a notification to the participant that they have to give reivew to the session owner
                    }
                    return RedirectToAction("GiveStudentReview", new { id = id });
                }
            }
            return RedirectToAction("Details", new { id = id });
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
            newSession.Points = session.Points;
            newSession.Status = session.Status;
            newSession.StudentName = studentName;
            newSession.LocationName = locationName;
            newSession.CategoryName = categoryName;
            return newSession;
        }

        private List<SelectListItem> DropDownHours()
        {
            List<SelectListItem> hours = new List<SelectListItem>();
            hours.Add(new SelectListItem
            {
                Value = "1",
                Text = "1",
            });
            hours.Add(new SelectListItem
            {
                Value = "2",
                Text = "2",
            });
            hours.Add(new SelectListItem
            {
                Value = "3",
                Text = "3",
            });
            hours.Add(new SelectListItem
            {
                Value = "4",
                Text = "4",
            });
            hours.Add(new SelectListItem
            {
                Value = "5",
                Text = "5",
            });
            return hours;
        }

        public ActionResult Create() //create a session page
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            ViewData["CategoryList"] = categoryContext.GetCategoryList(); //fills in category list 
            ViewData["LocationList"] = locationContext.GetLocationList(); //fills in location list
            ViewData["DropDownHours"] = DropDownHours();
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
            //System.Diagnostics.Debug.WriteLine(session);
            session.Status = 'N';
            if (ModelState.IsValid)
            {
                DateTime currenttime = DateTime.Now;
                TimeSpan ts = session.SessionDate - currenttime;
                if (ts.TotalHours < 2)
                {
                    ViewData["Error"] = "You cannot create a session 2 hour before now!";
                    return View(session);
                }
                session.SessionID = sessionContext.CreateSession(session);
                ViewData["Message"] = "Session Posted Successfully!";
                session.DateCreated = DateTime.Now;
                return RedirectToAction("UploadSessionPhoto", new { id = session.SessionID });

                //return RedirectToAction("UploadSessionPhoto");
            }
            else
            {
                ViewData["Error"] = "There is an invalid field. Please Try Again!";
                ViewData["DropDownHours"] = DropDownHours();
                return View(session);
            }
        }

        public ActionResult Cancel(int? id) //cancels the session if you're the session owner 
        {
            if ((HttpContext.Session.GetString("Role") == null) || //if the user is not logged in and tried to access the page, return to the error page
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            else if (id == null) //if the id cannot be found, return to the error page 
            {
                return RedirectToAction("Error", "Home");
            }
            else if (sessionContext.CheckSessionOwner(id, HttpContext.Session.GetInt32("StudentID")) == false) //check if the session is the owner of the session
            {
                return RedirectToAction("Error", "Home");
            }
            ViewData["CategoryList"] = categoryContext.GetCategoryList();
            ViewData["LocationList"] = locationContext.GetLocationList();
            Session session = sessionContext.GetSessionDetails(id);
            DateTime currenttime = DateTime.Now;
            TimeSpan ts = session.SessionDate - currenttime;
            if (ts.TotalHours < 2) //checks whether the session is 2 hours before
            {
                TempData["CannotCancel"] = "You cannot cancel any session 2 hours before it starts";
                return RedirectToAction("Details", new { id = session.SessionID });
            }
            List<StudentDetails> participantList = sessionContext.GetParticipantList(id);
            if (participantList.Count() > 0)
            {
                foreach (StudentDetails participant in participantList)
                {
                    notificationContext.AddSessionCancelNotification(participant.StudentID, HttpContext.Session.GetInt32("StudentID"), session.SessionID); //gives notification to the participant
                    int bookingid = sessionContext.GetBookingID(participant.StudentID, session.SessionID); //gets bookingoid
                    sessionContext.UpdateBookingStatus(bookingid);
                    sessionContext.UpdateSessionParticipant(session.Participants - 1, session.SessionID);
                }
            }
            session.Status = 'Y';
            sessionContext.UpdateSession(session);
            TempData["Cancel"] = "This session has been cancelled";
            return RedirectToAction("Details", new { id = session.SessionID });
        }

        //public ActionResult Edit(int? id)
        //{
        //    if ((HttpContext.Session.GetString("Role") == null) || //if the user is not logged in and tried to access the page, return to the error page
        //    (HttpContext.Session.GetString("Role") != "Student"))
        //    {
        //        return RedirectToAction("Error", "Home");
        //    }
        //    else if (id == null) //if the id cannot be found, return to the error page 
        //    {
        //        return RedirectToAction("Error", "Home");
        //    }
        //    else if (sessionContext.CheckSessionOwner(id, HttpContext.Session.GetInt32("StudentID")) == false) //check if the session is the owner of the session
        //    {
        //        return RedirectToAction("Error", "Home");
        //    }
        //    ViewData["CategoryList"] = categoryContext.GetCategoryList();
        //    ViewData["LocationList"] = locationContext.GetLocationList();
        //    Session session = sessionContext.GetSessionDetails(id);
        //    DateTime currenttime = DateTime.Now;
        //    TimeSpan ts = session.SessionDate - currenttime;
        //    if (ts.TotalHours < 2)
        //    {
        //        return RedirectToAction("Details", new { id = session.SessionID });
        //    }
        //    return View(session);
        //}

        //[HttpPost]
        //public ActionResult Edit(Session session)
        //{
        //    ViewData["CategoryList"] = categoryContext.GetCategoryList();
        //    ViewData["LocationList"] = locationContext.GetLocationList();
        //    DateTime currenttime = DateTime.Now;
        //    TimeSpan ts = session.SessionDate - currenttime;
        //    if (ts.TotalHours < 48)
        //    {
        //        TempData["OutofDate"] = "You cannot set a session date 2 days before or before today!";
        //        return RedirectToAction("Edit", new { id = session.SessionID });
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        sessionContext.UpdateSession(session);
        //        ViewData["Message"] = "Session Updated Successfully!";
        //        return View(session);
        //    }
        //    else
        //    {
        //        ViewData["Error"] = "There is an invalid field. Please Try Again!";
        //        return View(session);
        //    }

        //}

        public ActionResult MySession() //views session that user has created
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            List<Session> sessionList = sessionContext.GetMySession(HttpContext.Session.GetInt32("StudentID"));
            List<SessionViewModel> sessionDetailsList = MapToSessionVM(sessionList);
            return View(sessionDetailsList);
        }

        public ActionResult SessionJoined() //views sessions user has joined
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            ViewData["JoinedSession"] = "View Your Joined Sessions";
            List<Session> sessionList = sessionContext.GetSignedUpSession(HttpContext.Session.GetInt32("StudentID"));
            List<SessionViewModel> sessionDetailsList = MapToSessionVM(sessionList);
            return View(sessionDetailsList);
        }

        public ActionResult UploadSessionPhoto(int? id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            else if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }
            else if (sessionContext.CheckSessionOwner(id, HttpContext.Session.GetInt32("StudentID")) == false)
            {
                return RedirectToAction("Error", "Home");
            }
            Session session = sessionContext.GetSessionDetails(id);
            SessionPhoto newSession = MapToSingleSessionVM(session);
            return View(newSession);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSessionPhoto(SessionPhoto session)
        {
            if (session.FileToUpload != null && session.FileToUpload.Length > 0)
            {
                sessionContext.UpdateSessionPhoto(session.SessionID, session.Name);
                try
                { // Find the filename extension of the file to be uploaded.
                    string fileExt = Path.GetExtension(session.FileToUpload.FileName);
                    string uploadedFile = session.SessionID + session.Name + fileExt;
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
            TempData["Success"] = "Photo has been uploaded successfully";
            return RedirectToAction("Details", new { id = session.SessionID });
        }

    }
}