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

        public List<Session> GetAllSessions()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Session", conn);
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

        //public SessionViewModel GetSessionDetails(SessionViewModel session)
        //{
        //    SqlCommand cmd = new SqlCommand("SELECT * FROM Session WHERE SessionID = @selectedsessionid", conn);
        //    cmd.Parameters.AddWithValue("@selectedsessionid", session.SessionID);
        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    DataSet result = new DataSet();
        //    conn.Open();
        //    da.Fill(result, "SessionDetails");
        //    conn.Close();
        //    SessionViewModel sessionDetails = new SessionViewModel();
        //    if (result.Tables["SessionDetails"].Rows.Count > 0)
        //    {

        //    }

        //}
        public List<Session> GetMySession(int? studentID)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Session WHERE StudentID = @selectedstudentid", conn);
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
            SqlCommand cmd = new SqlCommand("INSERT INTO Session (SessionDate, Name, Description, Photo, Hours, Participants, StudentID, LocationID, CategoryID) " + 
                "OUTPUT INSERTED.SessionID " + 
                "VALUES(@sessiondate, @name, @description, @photo, @hours, @participants, @studentid, @locationid, @categoryid)", conn);
            cmd.Parameters.AddWithValue("@sessiondate", session.SessionDate);
            cmd.Parameters.AddWithValue("@name", session.Name);
            cmd.Parameters.AddWithValue("@description", session.Description);
            cmd.Parameters.AddWithValue("@photo", session.Photo);
            cmd.Parameters.AddWithValue("@hours", session.Hours);
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

