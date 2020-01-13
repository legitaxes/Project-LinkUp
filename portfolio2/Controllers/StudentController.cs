﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using portfolio2.DAL;
using portfolio2.Models;
using System.IO;
using System.Data.Common;

namespace portfolio2.Controllers
{
    public class StudentController : Controller
    {
        private StudentDAL studentContext = new StudentDAL();
        private CourseDAL courseContext = new CourseDAL();
        private StudentRatingDAL studentratingContext = new StudentRatingDAL();
        private RatingDAL ratingContext = new RatingDAL();
        private LocationDAL locationContext = new LocationDAL();
        private RequestDAL requestContext = new RequestDAL();

        // GET: Student Method
        public IActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
             (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }

            //if student never signup on our site before
            if (studentContext.checkStudent(HttpContext.Session.GetString("StudentNumber")) == false)
            {
                return RedirectToAction("Create");
            }

            //List<StudentDetails> studentList = studentContext.GetAllStudent();
            return View();
        }

        private List<SelectListItem> DropDownCourse()
        {
            List<SelectListItem> course = new List<SelectListItem>();
            List<Course> allcourselist = courseContext.getAllCourse();
            course.Add(new SelectListItem
            {
                Value = "",
                Text = "---Select Course---",
            });
            foreach (Course availablecourse in allcourselist)
            {
                course.Add(new SelectListItem
                {
                    Value = availablecourse.CourseID.ToString(),
                    Text = availablecourse.CourseName
                });
            }
            return course;
        }

        [HttpGet]
        public ActionResult Create()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            StudentDetails student = new StudentDetails();
            student.StudentNumber = HttpContext.Session.GetString("StudentNumber");
            student.Name = HttpContext.Session.GetString("LoginID");
            student.PhoneNo = null;
            ViewData["Courselist"] = DropDownCourse();
            //int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            student.Year = null;
            return View(student);
        }

        [HttpPost]
        public ActionResult Create(StudentDetails student)
        {
            student.Points = null;
            student.Photo = null;
            student.Name = HttpContext.Session.GetString("LoginID");
            student.StudentNumber = HttpContext.Session.GetString("StudentNumber");
            if (ModelState.IsValid)
            {
                ViewData["Message"] = "Student Profile Updated Successfully";
                student.StudentID = studentContext.Add(student);
                HttpContext.Session.SetInt32("StudentID", student.StudentID);
                //HttpContext.Session.SetString("Photo", student.Photo);
                ViewData["Courselist"] = DropDownCourse();
                return View(student);
            }
            ViewData["Message"] = "Something went wrong! Please try again!";
            ViewData["Courselist"] = DropDownCourse();
            return View(student);
        }

        [HttpGet]
        public ActionResult Update()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
               (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            StudentDetails student = studentContext.GetStudentDetails(HttpContext.Session.GetString("StudentNumber"));
            return View(student);
        }
        [HttpPost]
        public ActionResult Update(StudentDetails student)
        {
            if (ModelState.IsValid)
            {
                studentContext.Update(student);
                ViewData["Message"] = "Profile updated successfully!";
                return View(student);
            }
            ViewData["Message"] = "Could Not Update Profile. Please Try Again!";
            return View(student);
        }

        //view the details of the logged in user
        public ActionResult Details()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
               (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            List<StudentRating> studentratingList = studentratingContext.GetAllStudentRatings();
            List<Rating> ratingList = ratingContext.GetAllRatings();
            List<Review> reviewList = new List<Review>();
            foreach (StudentRating currentstudentrating in studentratingList)
            {
                if(currentstudentrating.StudentID == studentid)
                {
                    foreach(Rating currentrating in ratingList)
                    {
                        if (currentstudentrating.RatingID == currentrating.RatingID)
                        {
                            reviewList.Add(
                            new Review
                            {
                                Description = currentrating.Description,
                                RatingDate = currentrating.RatingDate,
                                Stars = currentrating.Stars
                            });
                        }
                    }
                }
            }
            ViewBag.List = reviewList;
            StudentDetails student = studentContext.GetStudentDetails(HttpContext.Session.GetString("StudentNumber"));
            StudentViewModel studentVM = MapToCourseAndRating(student);
            // StudentViewModel studentVM = MapToStudentVM(student); //to be completed - 1. student details 2. student course 3. student rating
            return View(studentVM);
        }

        public ActionResult ProfileDetails(string id) //profile detail page of another student, provided ID is given
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
            StudentDetails student = studentContext.GetStudentDetails(id);
            if(student == null)
                return RedirectToAction("Details");
            StudentViewModel studentVM = MapToCourseAndRating(student);
            return View(studentVM);
        }

        public ActionResult Photo()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
           (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            StudentPhoto studentPhoto = studentContext.GetPhotoDetails(studentid);
            return View(studentPhoto);
        }

        //Button on updating student profile photo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Photo(StudentPhoto student)
        {
            if (student.FileToUpload != null && student.FileToUpload.Length > 0)
            {
                try
                {
                    // Find the filename extension of the file to be uploaded.
                    string fileExt = Path.GetExtension(student.FileToUpload.FileName);
                    // Rename the uploaded file with the staff’s name.
                    string uploadedFile = student.Name + fileExt;
                    // Get the complete path to the images folder in server
                    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Profilepictures", uploadedFile);
                    // Upload the file to server
                    using (var fileSteam = new FileStream(savePath, FileMode.Create))
                    {
                        await student.FileToUpload.CopyToAsync(fileSteam);
                    }
                    student.Photo = uploadedFile;
                    studentContext.UploadPhoto(student);
                    HttpContext.Session.SetString("Photo", student.Photo);
                    ViewData["Message"] = "File uploaded successfully.";
                }
                catch (IOException)
                {
                    //File IO error, could be due to access rights denied 
                    ViewData["Message"] = "File uploading fail!";
                }
                catch (Exception ex)
                //Other type of error 
                {
                    ViewData["Message"] = ex.Message;
                }
            }
            return RedirectToAction("Details");
        }

        public StudentViewModel MapToCourseAndRating(StudentDetails student)
        {
            string coursename = "";
            int totalrating = 0;
            int amountofratings = 0;
            int averagerating = 0;

            List<Course> courseList = courseContext.getAllCourse();
            foreach (Course currentcourse in courseList)
            {
                if (currentcourse.CourseID == student.CourseID)
                {
                    coursename = currentcourse.CourseName;
                }
            }

            List<StudentRating> studentratingList = studentratingContext.GetAllStudentRatings();
            List<Rating> ratingList = ratingContext.GetAllRatings();
            foreach (StudentRating currentstudentrating in studentratingList)
            {
                if (currentstudentrating.StudentID == student.StudentID)
                {
                    foreach (Rating currentrating in ratingList)
                    {
                        if (currentrating.RatingID == currentstudentrating.RatingID)
                        {
                            totalrating = totalrating + currentrating.Stars;
                            amountofratings++;
                        }
                    }
                }
            }

            if (amountofratings > 0)
            {
                averagerating = totalrating / amountofratings;
            }

            else
            {
                averagerating = 0;
            }

            StudentViewModel studentVM = new StudentViewModel
            {
                StudentID = student.StudentID,
                Name = student.Name,
                Year = student.Year,
                StudentNumber = student.StudentNumber,
                Photo = student.Photo,
                PhoneNo = student.PhoneNo,
                Interest = student.Interest,
                ExternalLink = student.ExternalLink,
                Description = student.Description,
                Points = student.Points,
                CourseID = student.CourseID,
                CourseName = coursename,
                Rating = averagerating,
                TotalRatings = amountofratings,
        };
            return studentVM;
        }

        private List<SelectListItem> DropDownLocation()
        {
            List<SelectListItem> location = new List<SelectListItem>();
            List<Location> alllocationlist = locationContext.GetAllLocations();
            location.Add(new SelectListItem
            {
                Value = "",
                Text = "---Select Location---",
            });
            foreach (Location currentlocation in alllocationlist)
            {
                location.Add(new SelectListItem
                {
                    Value = currentlocation.LocationID.ToString(),
                    Text = currentlocation.LocationName,
                });
            }
            return location;
        }

        public ActionResult MakeRequest()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
                (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }            
            int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            int requestcounter = requestContext.GetNumberOfRequests(studentid);
            if (requestcounter >= 3)
            {
                return RedirectToAction("RequestRedirect", "Student");
            }
            ViewData["Locationlist"] = DropDownLocation();
            List<StudentDetails> studentList = studentContext.GetAllStudent();
            foreach (StudentDetails student in studentList)
                if (student.StudentID == studentid)
                {
                    int ID = student.StudentID;
                    ViewData["ID"] = ID;
                }
            return View();
        }
        
        [HttpPost]
        public ActionResult MakeRequest(Request request)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            int hours = Convert.ToInt32((request.AvailabilityTo - request.AvailabilityFrom).TotalHours);
            int points = hours * 10;
            request.DateRequest = DateTime.Now;
            request.PointsEarned = points;
            request.Status = 'N';
            request.StudentID = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            if (ModelState.IsValid && request.AvailabilityFrom < request.AvailabilityTo)
            {
                request.RequestID = requestContext.AddRequest(request);
                ViewData["Locationlist"] = DropDownLocation();
                return RedirectToAction("Myrequests", "Student");
            }
            else
            {
                ViewData["Error"] = "The session starting time can't be before the session ending time.";
            }
            ViewData["Locationlist"] = DropDownLocation();
            return View();
        }

        public ActionResult EditRequest(int? id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
           (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }

            Request request = requestContext.GetRequestByID(id.Value);

            if (request.StudentID != Convert.ToInt32(HttpContext.Session.GetInt32("StudentID")))
            { 
                return RedirectToAction("Error", "Home");
            }

            ViewData["Locationlist"] = DropDownLocation();

            if (request == null)
            {
                return RedirectToAction("Error", " Home");
            }

            return View(request);
        }

        // POST: Project/EditProject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRequest(Request request)
        {
            int hours = Convert.ToInt32((request.AvailabilityTo - request.AvailabilityFrom).TotalHours);
            int points = hours * 10;
            request.DateRequest = DateTime.Now;
            request.PointsEarned = points;
            request.Status = 'N';
            ViewData["Locationlist"] = DropDownLocation();
            if (ModelState.IsValid && request.AvailabilityFrom < request.AvailabilityTo)
            {
                requestContext.EditRequest(request);
                return RedirectToAction("Myrequests", "Student");
            }

            else
            {
                ViewData["Error"] = "The session starting time can't be before the session ending time.";
                return View(request);
            }
        }

        public ActionResult DeleteRequest(int? id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
           (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }

            Request request = requestContext.GetRequestByID(id.Value);
            if (request.StudentID != Convert.ToInt32(HttpContext.Session.GetInt32("StudentID")))
            {
                return RedirectToAction("Error", "Home");
            }
            ViewData["Locationlist"] = DropDownLocation();

            if (request == null)
            {
                return RedirectToAction("Error", " Home");
            }

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRequest(Request request)
        {
            requestContext.DeleteRequest(request.RequestID);
            return RedirectToAction("Myrequests", "Student");

        }

        public ActionResult MyRequests()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            List<Request> allrequestsList = requestContext.GetMyRequests(HttpContext.Session.GetInt32("StudentID"));
            List<RequestViewModel> allrequestviewmodelList = MapToStudentAndLocation(allrequestsList);
            return View(allrequestviewmodelList);
        }

        public ActionResult AllRequests()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            List<Request> allrequestsList = requestContext.GetAllRequests();
            List<RequestViewModel> allrequestviewmodelList= MapToStudentAndLocation(allrequestsList);
            return View(allrequestviewmodelList);
        }

        public List<RequestViewModel> MapToStudentAndLocation(List<Request> allrequestList)
        {
            string name = "";
            string locationname = "";
            List<StudentDetails> allstudentList = studentContext.GetAllStudent();
            List<Location> alllocationList = locationContext.GetAllLocations();
            List<RequestViewModel> requestVM = new List<RequestViewModel>();
            foreach (Request currentrequest in allrequestList)
            {
                foreach (StudentDetails currentstudent in allstudentList)
                {
                    if (currentstudent.StudentID == currentrequest.StudentID)
                    {
                        name = currentstudent.Name;
                        foreach (Location currentlocation in alllocationList)
                        {
                            if (currentlocation.LocationID == currentrequest.LocationID)
                            {
                                locationname = currentlocation.LocationName;
                            }
                        }
                    }
                }
                requestVM.Add(
                    new RequestViewModel
                {
                    RequestID = currentrequest.RequestID,
                    DateRequest = currentrequest.DateRequest,
                    Description = currentrequest.Description,
                    Title = currentrequest.Title,
                    AvailabilityFrom = currentrequest.AvailabilityFrom,
                    AvailabilityTo = currentrequest.AvailabilityTo,
                    PointsEarned = currentrequest.PointsEarned,
                    Status = currentrequest.Status,
                    LocationID = currentrequest.LocationID,
                    StudentID = currentrequest.StudentID,
                    Name = name,
                    LocationName = locationname,
                });
            }
            return requestVM;
        }

        public ActionResult RequestRedirect()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }

            return View();

        }
    }
}