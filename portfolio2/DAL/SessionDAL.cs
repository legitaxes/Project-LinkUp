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
    public class SessionDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        public SessionDAL()
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

        //Creates the booking record into the database
        public int CreateBooking(int sessionid, int hours)
        {
            int points = (15 * hours) / 2; //formula to be confirmed
            SqlCommand cmd = new SqlCommand("INSERT INTO Booking (PointsEarned, SessionID) " +
                "OUTPUT INSERTED.BookingID " + "VALUES(@points, @session)", conn);
            cmd.Parameters.AddWithValue("@points", points);
            cmd.Parameters.AddWithValue("@session", sessionid);
            conn.Open();
            int id = (int)cmd.ExecuteScalar();
            conn.Close();
            return id;
        }

        public bool CheckSignUp(int sessionid, int? studentid)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM StudentBooking sb INNER JOIN Booking b on sb.BookingID = b.BookingID " +
                "WHERE sb.StudentID = @selectedstudentid", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            if (studentid == null)
                return false;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "SessionJoined");
            conn.Close();
            foreach (DataRow row in result.Tables["SessionJoined"].Rows)
            {
                if (Convert.ToInt32(row["SessionID"]) == sessionid)
                {
                    return true;
                }
            }
            return false;
        }

        //Links the booking with the student 
        public int CreateStudentBooking(int? studentid, int bookingid)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO StudentBooking (StudentID, BookingID) "
                + "VALUES(@selectedstudentid, @selectedbookingid)", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            cmd.Parameters.AddWithValue("@selectedbookingid", bookingid);
            conn.Open();
            int? id = (int?)cmd.ExecuteScalar();
            conn.Close();
            return 0;
        }

        public int GetBookingID(int? studentid, int sessionid) //gets the bookingID based on the sessionID and studentID
        {
            SqlCommand cmd = new SqlCommand("Select b.BookingID from booking b INNER JOIN StudentBooking sb on sb.BookingID = b.BookingID" +
                " WHERE StudentID = @selectedstudentid AND b.SessionID = @selectedsessionid", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            cmd.Parameters.AddWithValue("@selectedsessionid", sessionid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "BookingIDList");
            conn.Close();
            int bookingID;
            foreach (DataRow row in result.Tables["BookingIDList"].Rows)
            {
                bookingID = Convert.ToInt32(row["BookingID"]);
                return bookingID;
            }
            return 0;
        }

        public int RemoveStudentBooking(int? studentid, int bookingid) //uses bookingID to remove the booking record StudentBooking
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM StudentBooking " +
                "WHERE StudentID = @selectedstudentid AND BookingID = @selectedbookingid", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            cmd.Parameters.AddWithValue("@selectedbookingid", bookingid);
            conn.Open();
            int count;
            count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }

        public int RemoveBooking(int bookingid)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM Booking " +
                "WHERE BookingID = @selectedbookingid", conn);
            cmd.Parameters.AddWithValue("@selectedbookingid", bookingid);
            conn.Open();
            int count;
            count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }

        public int UpdateSession(Session session)
        {
            SqlCommand cmd = new SqlCommand("UPDATE Session " +
                "SET SessionDate=@sessiondate, Name=@name, Description=@desc, Hours=@hours, LocationID=@location, CategoryID=@category" +
                " WHERE SessionID = @selectedsessionid", conn);
            cmd.Parameters.AddWithValue("@sessiondate", session.SessionDate);
            cmd.Parameters.AddWithValue("@name", session.Name);
            cmd.Parameters.AddWithValue("@desc", session.Description);
            cmd.Parameters.AddWithValue("@hours", session.Hours);
            cmd.Parameters.AddWithValue("@location", session.LocationID);
            cmd.Parameters.AddWithValue("@category", session.CategoryID);
            cmd.Parameters.AddWithValue("@selectedsessionid", session.SessionID);
            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }

        public void UpdateSessionParticipant(int participant, int sessionid)
        {
            SqlCommand cmd = new SqlCommand("UPDATE Session SET Participants = @participant" +
                " WHERE SessionID = @selectedsessionid", conn);
            cmd.Parameters.AddWithValue("@participant", participant);
            cmd.Parameters.AddWithValue("@selectedsessionid", sessionid);
            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
        }

        public List<Session> GetAllSessions()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Session ORDER BY DateCreated Desc", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "SessionList");
            conn.Close();
            List<Session> sessionList = new List<Session>();
            foreach (DataRow row in result.Tables["SessionList"].Rows)
            {
                string photo = "";
                if (!DBNull.Value.Equals(row["Photo"]))
                    photo = row["Photo"].ToString();
                else
                    photo = "stocksession.jpg";
                sessionList.Add(
                    new Session
                    {
                        SessionID = Convert.ToInt32(row["SessionID"]),
                        //DateCreated = Convert.ToDateTime(row["DateCreated"]),
                        SessionDate = Convert.ToDateTime(row["SessionDate"]),
                        Name = row["Name"].ToString(),
                        Description = row["Description"].ToString(),
                        Photo = photo,
                        Hours = Convert.ToInt32(row["Hours"]),
                        Participants = Convert.ToInt32(row["Participants"]),
                        Status = Convert.ToChar(row["Status"]),
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        LocationID = Convert.ToInt32(row["LocationID"]),
                        CategoryID = Convert.ToInt32(row["CategoryID"])
                    });
            }
            return sessionList;
        }

        public List<StudentDetails> GetParticipantList(int? sessionid) //queries for the list of participants that joined your session
        {
            SqlCommand cmd = new SqlCommand("Select * FROM Booking b INNER JOIN StudentBooking sb on sb.BookingID = b.BookingID " +
            "INNER JOIN Student s on s.StudentID = sb.StudentID " +
            "WHERE SessionID = @selectedsessionid", conn);
            cmd.Parameters.AddWithValue("@selectedsessionid", sessionid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "StudentDetails");
            conn.Close();
            List<StudentDetails> studentDetailsList = new List<StudentDetails>();
            foreach (DataRow row in result.Tables["StudentDetails"].Rows)
            {
                studentDetailsList.Add(
                    new StudentDetails
                    {
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        Name = row["Name"].ToString(),
                        Year = Convert.ToInt32(row["Year"]),
                        StudentNumber = row["StudentNo"].ToString(),
                        Photo = row["Photo"].ToString(),
                        PhoneNo = Convert.ToInt32(row["PhoneNo"]),
                        Interest = row["Interest"].ToString(),
                        ExternalLink = row["ExternalLink"].ToString(),
                        Description = row["Description"].ToString(),
                        //Points = Convert.ToInt32(row["Points"]),
                        CourseID = Convert.ToInt32(row["CourseID"])
                    });
            }
            return studentDetailsList;
        }

        public bool CheckSessionOwner(int? sessionID, int? studentID) //checks whether the session is hosted by the user, true = yes, false = no
        {
            if (sessionID == null)
            {
                return false;
            }
            if (studentID == null)
            {
                return false;
            }
            SqlCommand cmd = new SqlCommand("SELECT * FROM Session WHERE SessionID = @selectedsessionid AND StudentID = @selectedstudentid", conn);
            cmd.Parameters.AddWithValue("@selectedsessionid", sessionID);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "SessionDetails");
            conn.Close();
            Session sessionDetails = new Session();
            if (result.Tables["SessionDetails"].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int UpdateSessionPhoto(int sessionid, string sessionname)
        {
            SqlCommand cmd = new SqlCommand("UPDATE Session SET Photo=@sessionname" + 
                " WHERE SessionID = @selectedsessionid", conn);
            cmd.Parameters.AddWithValue("@sessionname", sessionid+sessionname + ".jpg");
            cmd.Parameters.AddWithValue("@selectedsessionid", sessionid);
            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }

        public Session GetSessionDetails(int? sessionID) //queries for the details of a session
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Session WHERE SessionID = @selectedsessionid", conn);
            cmd.Parameters.AddWithValue("@selectedsessionid", sessionID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "SessionDetails");
            conn.Close();
            Session sessionDetails = new Session();
            if (result.Tables["SessionDetails"].Rows.Count > 0)
            {
                DataTable table = result.Tables["SessionDetails"];
                if (!DBNull.Value.Equals(table.Rows[0]["SessionID"]))
                    sessionDetails.SessionID = Convert.ToInt32(table.Rows[0]["SessionID"]);
                if (!DBNull.Value.Equals(table.Rows[0]["DateCreated"]))
                    sessionDetails.DateCreated = Convert.ToDateTime(table.Rows[0]["DateCreated"]);
                if (!DBNull.Value.Equals(table.Rows[0]["SessionDate"]))
                    sessionDetails.SessionDate = Convert.ToDateTime(table.Rows[0]["SessionDate"]);
                if (!DBNull.Value.Equals(table.Rows[0]["Name"]))
                    sessionDetails.Name = table.Rows[0]["Name"].ToString();
                if (!DBNull.Value.Equals(table.Rows[0]["Description"]))
                    sessionDetails.Description = table.Rows[0]["Description"].ToString();
                if (!DBNull.Value.Equals(table.Rows[0]["Photo"]))
                    sessionDetails.Photo = table.Rows[0]["Photo"].ToString();
                if (!DBNull.Value.Equals(table.Rows[0]["Hours"]))
                    sessionDetails.Hours = Convert.ToInt32(table.Rows[0]["Hours"]);
                if (!DBNull.Value.Equals(table.Rows[0]["Participants"]))
                    sessionDetails.Participants = Convert.ToInt32(table.Rows[0]["Participants"]);
                if (!DBNull.Value.Equals(table.Rows[0]["Status"]))
                    sessionDetails.Status = Convert.ToChar(table.Rows[0]["Status"]);
                if (!DBNull.Value.Equals(table.Rows[0]["StudentID"]))
                    sessionDetails.StudentID = Convert.ToInt32(table.Rows[0]["StudentID"]);
                if (!DBNull.Value.Equals(table.Rows[0]["LocationID"]))
                    sessionDetails.LocationID = Convert.ToInt32(table.Rows[0]["LocationID"]);
                if (!DBNull.Value.Equals(table.Rows[0]["CategoryID"]))
                    sessionDetails.CategoryID = Convert.ToInt32(table.Rows[0]["CategoryID"]);
                return sessionDetails;
            }
            else
            {
                return null;
            }
        }

        public List<Session> GetSignedUpSession(int? studentID) //queries for session the user have signed up
        {
            SqlCommand cmd = new SqlCommand("SELECT s.SessionID, s.SessionDate, s.Name, s.Description, s.Photo, s.Hours, s.Participants, s.Points, s.Status, s.StudentID, s.LocationID, s.CategoryID " +
                "FROM StudentBooking sb INNER JOIN Booking b ON sb.BookingID = b.BookingID " +
                "INNER JOIN Session s ON s.SessionID = b.SessionID " +
                "WHERE sb.StudentID = @selectedstudentID", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "SignedUpSessionList");
            conn.Close();
            List<Session> sessionList = new List<Session>();
            foreach (DataRow row in result.Tables["SignedUpSessionList"].Rows)
            {
                string photo = "";
                if (!DBNull.Value.Equals(row["Photo"]))
                    photo = row["Photo"].ToString();
                else
                    photo = "stocksession.jpg";
                sessionList.Add(
                    new Session
                    {
                        SessionID = Convert.ToInt32(row["SessionID"]),
                        SessionDate = Convert.ToDateTime(row["SessionDate"]),
                        Name = row["Name"].ToString(),
                        Description = row["Description"].ToString(),
                        Photo = photo,
                        Hours = Convert.ToInt32(row["Hours"]),
                        Participants = Convert.ToInt32(row["Participants"]),
                        Points = Convert.ToInt32(row["Points"]),
                        Status = Convert.ToChar(row["Status"]),
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        LocationID = Convert.ToInt32(row["LocationID"]),
                        CategoryID = Convert.ToInt32(row["CategoryID"])
                    });
            }
            return sessionList;
        }

        public List<Session> GetMySession(int? studentID)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Session WHERE StudentID = @selectedstudentID", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "MySessionList");
            conn.Close();
            List<Session> sessionList = new List<Session>();
            foreach (DataRow row in result.Tables["MySessionList"].Rows)
            {
                string photo = "";
                if (!DBNull.Value.Equals(row["Photo"]))
                    photo = row["Photo"].ToString();
                else
                    photo = "stocksession.jpg";
                sessionList.Add(
                    new Session
                    {
                        SessionID = Convert.ToInt32(row["SessionID"]),
                        SessionDate = Convert.ToDateTime(row["SessionDate"]),
                        Name = row["Name"].ToString(),
                        Description = row["Description"].ToString(),
                        Photo = photo,
                        Hours = Convert.ToInt32(row["Hours"]),
                        Participants = Convert.ToInt32(row["Participants"]),
                        Points = Convert.ToInt32(row["Points"]),
                        Status = Convert.ToChar(row["Status"]),
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        LocationID = Convert.ToInt32(row["LocationID"]),
                        CategoryID = Convert.ToInt32(row["CategoryID"])
                    });
            }
            return sessionList;
        }

        public int CreateSession(Session session)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Session (SessionDate, Name, Description, Photo, Hours, Points, Participants, StudentID, LocationID, CategoryID) " + 
                "OUTPUT INSERTED.SessionID " + 
                "VALUES(@sessiondate, @name, @description, @photo, @hours, @points, @participants, @studentid, @locationid, @categoryid)", conn);
            cmd.Parameters.AddWithValue("@sessiondate", session.SessionDate);
            cmd.Parameters.AddWithValue("@name", session.Name);
            cmd.Parameters.AddWithValue("@description", session.Description);
            cmd.Parameters.AddWithValue("@photo", session.Photo);
            cmd.Parameters.AddWithValue("@hours", session.Hours);
            cmd.Parameters.AddWithValue("@points", (15 * session.Hours) / 2);
            cmd.Parameters.AddWithValue("@participants", session.Participants);
            cmd.Parameters.AddWithValue("@studentid", session.StudentID);
            cmd.Parameters.AddWithValue("@locationid", session.LocationID);
            cmd.Parameters.AddWithValue("@categoryid", session.CategoryID);
            conn.Open();
            session.SessionID = (int)cmd.ExecuteScalar();
            conn.Close();
            return session.SessionID;
        }

        public List<Session> FilteredSession(int? categoryID)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Session WHERE CategoryID = @selectedcategoryID", conn);
            cmd.Parameters.AddWithValue("@selectedcategoryID", categoryID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "SessionList");
            conn.Close();
            List<Session> sessionList = new List<Session>();
            foreach (DataRow row in result.Tables["SessionList"].Rows)
            {
                string photo = "";
                if (!DBNull.Value.Equals(row["Photo"]))
                    photo = row["Photo"].ToString();
                else
                    photo = "stocksession.jpg";
                sessionList.Add(
                    new Session
                    {
                        SessionID = Convert.ToInt32(row["SessionID"]),
                        SessionDate = Convert.ToDateTime(row["SessionDate"]),
                        Name = row["Name"].ToString(),
                        Description = row["Description"].ToString(),
                        Photo = photo,
                        Hours = Convert.ToInt32(row["Hours"]),
                        Participants = Convert.ToInt32(row["Participants"]),
                        Status = Convert.ToChar(row["Status"]),
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        LocationID = Convert.ToInt32(row["LocationID"]),
                        CategoryID = Convert.ToInt32(row["CategoryID"])
                    });
            }
            return sessionList;
        }

    }
}

