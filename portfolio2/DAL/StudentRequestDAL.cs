using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using portfolio2.Models;
using Microsoft.AspNetCore.Http;

namespace portfolio2.DAL
{
    public class StudentRequestDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        //Constructor
        public StudentRequestDAL()
        {
            //Locate the appsettings.json file
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            //Read ConnectionString from appsettings.json file
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "LinkupConnectionString");

            //Instantiate a SqlConnection object with the
            //Connection String read.
            conn = new SqlConnection(strConn);
        }

        public int GetNumberOfParticipants(int requestid)
        {
            SqlCommand cmd = new SqlCommand(
           "SELECT COUNT(*) FROM StudentRequest WHERE RequestID = @selectedrequestid", conn);
            cmd.Parameters.AddWithValue("@selectedrequestid", requestid);
            conn.Open();
            int rowsAmount = (int)cmd.ExecuteScalar();
            conn.Close();
            return rowsAmount;
        }

        public List<StudentRequest> GetAllStudentRequests()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM StudentRequest", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "StudentRequestList");
            conn.Close();
            List<StudentRequest> studentrequestList = new List<StudentRequest>();
            foreach (DataRow row in result.Tables["StudentRequestList"].Rows)
            {
                studentrequestList.Add(
                    new StudentRequest
                    {
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        RequestID = Convert.ToInt32(row["RequestID"])
                    });
            }
            return studentrequestList;
        }

        public int AddStudentRequest(int studentid, int requestid)
        {
            SqlCommand cmd = new SqlCommand
            ("INSERT INTO StudentRequest (StudentID, RequestID)" +
            " VALUES(@studentid, @requestid)", conn);
            cmd.Parameters.AddWithValue("@studentid", studentid);
            cmd.Parameters.AddWithValue("@requestid", requestid);
            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }

        public int ConvertRequestToSession(Request request, int tutorstudentid)
        {
            SqlCommand cmd = new SqlCommand
            ("INSERT INTO SESSION (DateCreated, SessionDate, Name, Description, Photo, Hours, Participants, Points, Status, StudentID, LocationID, CategoryID)" +
            " OUTPUT INSERTED.SessionID" +
            " VALUES(@daterequest, @availablefrom, @title, @description, @photo, @hours, @participants, @pointsearned, @status, @tutorstudentid, @locationid, @categoryid)", conn);
            cmd.Parameters.AddWithValue("@daterequest", request.DateRequest);
            cmd.Parameters.AddWithValue("@availablefrom", request.AvailabilityFrom);
            cmd.Parameters.AddWithValue("@title", request.Title);
            cmd.Parameters.AddWithValue("@description", request.Description);
            cmd.Parameters.AddWithValue("@photo", request.Photo);
            cmd.Parameters.AddWithValue("@hours", request.Hours);
            int participants = 1;
            cmd.Parameters.AddWithValue("@participants", participants);
            cmd.Parameters.AddWithValue("@pointsearned", request.PointsEarned);
            cmd.Parameters.AddWithValue("@status", request.Status);
            cmd.Parameters.AddWithValue("@tutorstudentid", tutorstudentid);
            cmd.Parameters.AddWithValue("@locationid", request.LocationID);
            cmd.Parameters.AddWithValue("@categoryid", request.CategoryID);
            conn.Open();
            int sessionid = (int)cmd.ExecuteScalar();
            conn.Close();
           
            return sessionid;
        }

        public int AddConversionToBooking(Request request, int sessionid)
        {
            SqlCommand cmd2 = new SqlCommand
            ("INSERT INTO Booking (PointsEarned, SessionID)" +
            " OUTPUT INSERTED.BookingID" +
            " VALUES(@pointsearned, @sessionid)", conn);
            cmd2.Parameters.AddWithValue("@pointsearned", request.PointsEarned);
            cmd2.Parameters.AddWithValue("@sessionid", sessionid);
            conn.Open();
            int bookingid = (int)cmd2.ExecuteScalar();
            conn.Close();
            return bookingid;
        }

        public void AddConversionToStudentBooking(Request request, int bookingid)
        {
            SqlCommand cmd3 = new SqlCommand
("INSERT INTO StudentBooking (StudentID, BookingID)" +
" VALUES(@studentid, @bookingid)", conn);
            cmd3.Parameters.AddWithValue("@studentid", request.StudentID);
            cmd3.Parameters.AddWithValue("@bookingid", bookingid);
            conn.Open();
            cmd3.ExecuteScalar();
            conn.Close();

        }
        public int UpdateConversionStatus(Request request)
        {
            SqlCommand cmd4 = new SqlCommand
    ("UPDATE Request" +
    " SET Status = 'Y'" +
    " WHERE RequestID = @requestid", conn);
            cmd4.Parameters.AddWithValue("@requestid", request.RequestID);
            conn.Open();
            int count = cmd4.ExecuteNonQuery();
            conn.Close();
            return count;
        }
    }
}
