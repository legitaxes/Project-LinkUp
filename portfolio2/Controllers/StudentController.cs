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
        private StudentRequestDAL studentrequestContext = new StudentRequestDAL();
        private CategoryDAL categoryContext = new CategoryDAL();

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
      
        private List<SelectListItem> DropDownCategory()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            List<Category> allcategorylist = categoryContext.GetAllCategory();
            category.Add(new SelectListItem
            {
                Value = "",
                Text = "---Select Category---",
            });
            foreach (Category availablecategory in allcategorylist)
            {
                category.Add(new SelectListItem
                {
                    Value = availablecategory.CategoryID.ToString(),
                    Text = availablecategory.CategoryName
                });
            }
            return category;
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
                return RedirectToAction("StudentMain", "Home");
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
                                Stars = currentrating.Stars,
                                RatingType = currentrating.RatingType
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

        public ActionResult StudentList(string searchedvalue)
        {
            List<StudentDetails> studentList = studentContext.GetSearchedStudent(searchedvalue);
            ViewData["SearchedValue"] = searchedvalue;
            if (studentList != null)
            {
                return View(studentList);
            }
            else
            {
                return View();
            }
        }

        public ActionResult ProfileDetails(string id) //profile detail page of another student, provided ID is given
        {
           // if ((HttpContext.Session.GetString("Role") == null) ||
           //(HttpContext.Session.GetString("Role") != "Student"))
           // {
           //     return RedirectToAction("Error", "Home");
           // }
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }
            StudentDetails student = studentContext.GetStudentDetails(id);
            if (student == null)
            {
                return RedirectToAction("Error", "Home");
            }
            int studentid = student.StudentID;
            List<StudentRating> studentratingList = studentratingContext.GetAllStudentRatings();
            List<Rating> ratingList = ratingContext.GetAllRatings();
            List<Review> reviewList = new List<Review>();
            foreach (StudentRating currentstudentrating in studentratingList)
            {
                if (currentstudentrating.StudentID == studentid)
                {
                    foreach (Rating currentrating in ratingList)
                    {
                        if (currentstudentrating.RatingID == currentrating.RatingID)
                        {
                            reviewList.Add(
                            new Review
                            {
                                Description = currentrating.Description,
                                RatingDate = currentrating.RatingDate,
                                Stars = currentrating.Stars,
                                RatingType = currentrating.RatingType
                            });
                        }
                    }
                }
            }
            ViewBag.List = reviewList;

            if (student == null)
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
            return RedirectToAction("Update");
        }


        public StudentViewModel MapToCourseAndRating(StudentDetails student)
        {
            string coursename = "";
            double totalrating = 0;
            double amountofratings = 0;
            double averagerating = 0;

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
            ViewData["Hourlist"] = DropDownHours();
            ViewData["Locationlist"] = DropDownLocation();
            ViewData["Categorylist"] = DropDownCategory();
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
            int hours = Convert.ToInt32(request.Hours);
            int points = (hours * 15)/2;
            request.DateRequest = DateTime.Now;
            request.PointsEarned = points;
            request.Status = 'N';
            request.StudentID = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            if (ModelState.IsValid)
            {
                request.RequestID = Convert.ToInt32(requestContext.AddRequest(request));
                ViewData["Locationlist"] = DropDownLocation();
                return RedirectToAction("Myrequests", "Student");
            }
            ViewData["Hourlist"] = DropDownHours();
            ViewData["Locationlist"] = DropDownLocation();
            ViewData["Categorylist"] = DropDownCategory();
            return View();
        }

        /*public ActionResult EditRequest(int? id)
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

            ViewData["Hourlist"] = DropDownHours();
            List<SelectListItem> maxcapList = new List<SelectListItem>();
            maxcapList = DropDownMaxCap();
            int participantcount = studentrequestContext.GetNumberOfParticipants(request.RequestID);
            for (int i = 0; i < participantcount; i++)
            {
                maxcapList.RemoveAt(0);
            }
            ViewData["MaxCaplist"] = maxcapList;
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
            int hours = Convert.ToInt32(request.Hours);
            int points = (hours * 15) / 2;
            request.PointsEarned = points;
            request.Status = 'N';
          
            if (ModelState.IsValid)
            {
                requestContext.EditRequest(request);
                return RedirectToAction("Myrequests", "Student");
            }
            return View(request);
        }*/

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
            DateTime currenttime = DateTime.Now;
            TimeSpan ts = request.AvailabilityFrom - currenttime;
            double hours = ts.TotalHours;
            if (hours < 48)
            {
                return RedirectToAction("DeleteRedirect", "Student");
            }
            ViewData["Hourlist"] = DropDownHours();
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
            ViewData["Hourlist"] = DropDownHours();
            ViewData["Locationlist"] = DropDownLocation();
            requestContext.DeleteRequest(request.RequestID);
            return RedirectToAction("Myrequests", "Student");

        }

        //public ActionResult LeaveRequest(int? id)
        //{
        //    if ((HttpContext.Session.GetString("Role") == null) ||
        //   (HttpContext.Session.GetString("Role") != "Student"))
        //    {
        //        return RedirectToAction("Error", "Home");
        //    }

        //    Request request = requestContext.GetRequestByID(id.Value);
        //    if (request.RequestID != id)
        //    {
        //        return RedirectToAction("Error", "Home");
        //    }
        //    DateTime currenttime = DateTime.Now;
        //    TimeSpan ts = request.AvailabilityFrom - currenttime;
        //    double hours = ts.TotalHours;
        //    if (hours < 48)
        //    {
        //        return RedirectToAction("DeleteRedirect", "Student");
        //    }
        //    ViewData["Hourlist"] = DropDownHours();
        //    ViewData["Locationlist"] = DropDownLocation();

        //    if (request == null)
        //    {
        //        return RedirectToAction("Error", " Home");
        //    }

        //    return View(request);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LeaveRequest(Request request)
        //{
        //    ViewData["Hourlist"] = DropDownHours();
        //    ViewData["Locationlist"] = DropDownLocation();
        //    requestContext.DeleteStudentRequest(Convert.ToInt32(HttpContext.Session.GetInt32("StudentID")), request.RequestID);
        //    return RedirectToAction("Myrequests", "Student");
        //}

        public ActionResult MyRequests()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            List<Request> allrequestsList = requestContext.GetMyRequests(HttpContext.Session.GetInt32("StudentID"));
            if (allrequestsList.Count() == 0)
            {
                ViewData["MyRequestEmpty"] = "It does not seem like you have created any request!";
            }
            List<RequestViewModel> allrequestviewmodelList = MapToStudentAndLocation(allrequestsList);
            List<JoinedRequests> myjoinedrequestsList = requestContext.GetMyJoinedRequests(studentid);
            if(myjoinedrequestsList.Count == 0)
            {
                ViewData["JoinedRequestEmpty"] = "It doesn't seem like you have joined any request...";
            }
            ViewBag.List = myjoinedrequestsList;
            return View(allrequestviewmodelList);
        }

        public ActionResult AllRequests()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            List<Request> allrequestsList = requestContext.GetAllRequestNotCompleted();
            if (allrequestsList.Count() == 0)
            {
                ViewData["MyRequestEmpty"] = "It does not seem like there is any request right now";
            }
            List<RequestViewModel> allrequestviewmodelList= MapToStudentAndLocation(allrequestsList);

            return View(allrequestviewmodelList);
        }

        public List<RequestViewModel> MapToStudentAndLocation(List<Request> allrequestList)
        {
            string name = "";
            string locationname = "";
            string categoryname = "";
            List<StudentRequest> allstudentRequestList = studentrequestContext.GetAllStudentRequests();
            List<StudentDetails> allstudentList = studentContext.GetAllStudent();
            List<Category> allcategoryList = categoryContext.GetAllCategory();
            List<Location> alllocationList = locationContext.GetAllLocations();
            List<RequestViewModel> requestVM = new List<RequestViewModel>();
            foreach (Request currentrequest in allrequestList)
            {
                int participantcount = 1;
                foreach (StudentRequest currentstudentrequest in allstudentRequestList)
                {
                    if (currentstudentrequest.RequestID == currentrequest.RequestID)
                    {
                        participantcount += 1;
                    }
                }
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
                foreach (Category currentcategory in allcategoryList)
                {
                    if (currentcategory.CategoryID == currentrequest.CategoryID)
                    {
                        categoryname = currentcategory.CategoryName;
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
                    Hours = currentrequest.Hours,
                    CurrCap = participantcount,
                    PointsEarned = currentrequest.PointsEarned,
                    Status = currentrequest.Status,
                    LocationID = currentrequest.LocationID,
                    StudentID = currentrequest.StudentID,
                    Name = name,
                    LocationName = locationname,
                    CategoryID = currentrequest.CategoryID,
                    CategoryName = categoryname
                });
            }
            return requestVM;
        }

        public ActionResult JoinRequest(int? id)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            Request request = requestContext.GetRequestByID(id.Value);

            ViewData["Hourlist"] = DropDownHours();
            ViewData["Locationlist"] = DropDownLocation();
            ViewData["Categorylist"] = DropDownCategory();
            if (request.Status == 'Y')
            {
                return RedirectToAction("Error", " Home");
            }
            if (studentid == request.StudentID)
            {
                return RedirectToAction("RequestRedirect", "Student");
            }
            

            if (request == null)
            {
                return RedirectToAction("Error", " Home");
            }

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JoinRequest(Request request)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
           (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));

            if (studentid == request.StudentID)
            {
                return RedirectToAction("RequestRedirect", "Student");
            }

            if (request.Status == 'Y')
            {
                return RedirectToAction("Error", " Home");
            }

            if (request == null)
            {
                return RedirectToAction("Error", " Home");
            }

            if (studentid != request.StudentID)
            {
                int sessionid = studentrequestContext.ConvertRequestToSession(request, studentid);
                int bookingid = studentrequestContext.AddConversionToBooking(request, sessionid);
                studentrequestContext.AddConversionToStudentBooking(request, bookingid);
                studentrequestContext.AddStudentRequest(studentid, request.RequestID);
                studentrequestContext.UpdateConversionStatus(request);
            }

            return RedirectToAction("AllRequests", "Student");
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

        public ActionResult DeleteRedirect()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            return View();
        }

        public ActionResult StudentLeaderBoard()
        {
            List<StudentDetails> studentList = new List<StudentDetails>();
            studentList = studentContext.GetPoints();
            ViewBag.List = studentList;
            return View();

        }
    }
}