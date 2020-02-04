using System;
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
    public class RequestController : Controller
    {
        private StudentDAL studentContext = new StudentDAL();
        private CourseDAL courseContext = new CourseDAL();
        private StudentRatingDAL studentratingContext = new StudentRatingDAL();
        private RatingDAL ratingContext = new RatingDAL();
        private LocationDAL locationContext = new LocationDAL();
        private RequestDAL requestContext = new RequestDAL();
        private StudentRequestDAL studentrequestContext = new StudentRequestDAL();
        private NotificationDAL notificationContext = new NotificationDAL();
        private CategoryDAL categoryContext = new CategoryDAL();

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
                return RedirectToAction("RequestRedirect", "Request");
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
            int points = (hours * 15) / 2;
            request.DateRequest = DateTime.Now;
            request.PointsEarned = points;
            request.Status = 'N';
            request.Photo = "stocksession.jpg";
            request.StudentID = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            if (ModelState.IsValid)
            {
                request.RequestID = Convert.ToInt32(requestContext.AddRequest(request));
                ViewData["Locationlist"] = DropDownLocation();
                return RedirectToAction("UploadRequestPhoto", new { requestid = request.RequestID });
            }
            ViewData["Hourlist"] = DropDownHours();
            ViewData["Locationlist"] = DropDownLocation();
            ViewData["Categorylist"] = DropDownCategory();
            return View();
        }

        public ActionResult UploadRequestPhoto(int requestid)
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            if (requestid == null)
            {
                return RedirectToAction("Error", "Home");
            }
            Request request = requestContext.GetRequestByID(requestid);
            if (request == null)
            {
                return RedirectToAction("Error", "Home");
            }
            if (request.StudentID != HttpContext.Session.GetInt32("StudentID"))
            {
                return RedirectToAction("Error", "Home");
            }
            RequestPhoto requestphoto = MapToRequestPhoto(request);

            return View(requestphoto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadRequestPhoto(RequestPhoto requestphoto)
        {
            if (requestphoto.FileToUpload != null && requestphoto.FileToUpload.Length > 0)
            {
                try
                { // Find the filename extension of the file to be uploaded.
                    string fileExt = Path.GetExtension(requestphoto.FileToUpload.FileName);
                    string uploadedFile = requestphoto.RequestID + requestphoto.Title + fileExt;
                    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\sessions", uploadedFile);
                    using (var fileSteam = new FileStream(savePath, FileMode.Create))
                    {
                        await requestphoto.FileToUpload.CopyToAsync(fileSteam);
                    }
                    requestphoto.Photo = uploadedFile;
                    requestContext.UploadRequestPhoto(requestphoto);
                    ViewData["Message"] = "File uploaded successfully.";
                }
                catch (IOException)
                {
                    ViewData["Message"] = "File uploading failed!";
                }
                catch (Exception ex)
                {
                    ViewData["Message"] = ex.Message;
                }
            }
            TempData["Success"] = "Photo has been uploaded successfully";
            return RedirectToAction("MyRequests", "Request");
        }

        public RequestPhoto MapToRequestPhoto(Request request)
        {
            string title = "";
            string description = "";
            string photo = "";
            List<Request> allrequestList = requestContext.GetAllRequestNotCompleted();
            int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            foreach (Request currentrequest in allrequestList)
            {
                if (request.RequestID == currentrequest.RequestID && currentrequest.StudentID == studentid) 
                {
                    title = currentrequest.Title;
                    description = currentrequest.Description;
                    photo = currentrequest.Photo;
                    break;
                }
            }
            RequestPhoto newrequestphoto = new RequestPhoto();
            newrequestphoto.RequestID = request.RequestID;
            newrequestphoto.Title = title;
            newrequestphoto.Photo = photo;
            newrequestphoto.Description = description;

            return newrequestphoto;
        }

        public ActionResult EditRequest(int? id)
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
            Request request = requestContext.GetRequestByID(id.Value);
            if (request == null)
            {
                return RedirectToAction("Error", "Home");
            }
            if (request.StudentID != Convert.ToInt32(HttpContext.Session.GetInt32("StudentID")))
            { 
                return RedirectToAction("Error", "Home");
            }

            ViewData["Hourlist"] = DropDownHours();
            ViewData["Categorylist"] = DropDownCategory();
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
                return RedirectToAction("Myrequests", "Request");
            }
            return View(request);
        }

        public ActionResult DeleteRequest(int? id)
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
            Request request = requestContext.GetRequestByID(id.Value);
            if (request == null)
            {
                return RedirectToAction("Error", "Home");
            }
            if (request.StudentID != Convert.ToInt32(HttpContext.Session.GetInt32("StudentID")))
            {
                return RedirectToAction("Error", "Home");
            }

            ViewData["Hourlist"] = DropDownHours();
            ViewData["Locationlist"] = DropDownLocation();

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRequest(Request request)
        {
            ViewData["Hourlist"] = DropDownHours();
            ViewData["Locationlist"] = DropDownLocation();
            requestContext.DeleteRequest(request.RequestID);
            return RedirectToAction("Myrequests", "Request");

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
            if (myjoinedrequestsList.Count == 0)
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
            List<RequestViewModel> allrequestviewmodelList = MapToStudentAndLocation(allrequestsList);

            return View(allrequestviewmodelList);
        }

        public List<RequestViewModel> MapToStudentAndLocation(List<Request> allrequestList)
        {
            string name = "";
            string locationname = "";
            string categoryname = "";
            string photo = "";
            List<StudentRequest> allstudentRequestList = studentrequestContext.GetAllStudentRequests();
            List<StudentDetails> allstudentList = studentContext.GetAllStudent();
            List<Category> allcategoryList = categoryContext.GetAllCategory();
            List<Location> alllocationList = locationContext.GetAllLocations();
            List<RequestViewModel> requestVM = new List<RequestViewModel>();
            foreach (Request currentrequest in allrequestList)
            {
                photo = currentrequest.Photo;
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
                        Photo = photo,
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
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }
            int studentid = Convert.ToInt32(HttpContext.Session.GetInt32("StudentID"));
            Request request = requestContext.GetRequestByID(id.Value);
            if (request == null)
            {
                return RedirectToAction("Error", "Home");
            }
            ViewData["Hourlist"] = DropDownHours();
            ViewData["Locationlist"] = DropDownLocation();
            ViewData["Categorylist"] = DropDownCategory();
            if (request.Status == 'Y')
            {
                return RedirectToAction("Error", " Home");
            }
            if (studentid == request.StudentID)
            {
                return RedirectToAction("Error", "Home");
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
                return RedirectToAction("Error", "Home");
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
                notificationContext.AddJoinedRequestNotification(studentid, sessionid, request.StudentID);
            }

            return RedirectToAction("AllRequests", "Request");
        }

        public ActionResult RequestRedirect()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Student"))
            {
                return RedirectToAction("Error", "Home");
            }
            TempData["Message"] = "You can only create up to 3 requests!";
            return RedirectToAction("AllRequests");
        }
    }
}