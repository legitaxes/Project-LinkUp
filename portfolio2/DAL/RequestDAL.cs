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
    public class RequestDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;

        public RequestDAL()
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

        public int AddRequest(Request request)
        {
            SqlCommand cmd = new SqlCommand
            ("INSERT INTO Request (DateRequest, Description, Title, AvailabilityFrom, Hours, PointsEarned, Status, LocationID, StudentID, CategoryID)" +
            " OUTPUT INSERTED.RequestID" +
            " VALUES(@daterequest, @description, @title, @availabilityfrom, @hours, @pointsearned, @status, @locationid, @studentid, @categoryid)", conn);
            cmd.Parameters.AddWithValue("@daterequest", request.DateRequest);
            cmd.Parameters.AddWithValue("@description", request.Description);
            cmd.Parameters.AddWithValue("@title", request.Title);
            cmd.Parameters.AddWithValue("@availabilityfrom", request.AvailabilityFrom);
            cmd.Parameters.AddWithValue("@hours", request.Hours);
            cmd.Parameters.AddWithValue("@pointsearned", request.PointsEarned);
            cmd.Parameters.AddWithValue("@status", request.Status);
            cmd.Parameters.AddWithValue("@locationid", request.LocationID);
            cmd.Parameters.AddWithValue("@studentid", request.StudentID);
            cmd.Parameters.AddWithValue("@categoryid", request.CategoryID);
            conn.Open();
            request.RequestID = (int)cmd.ExecuteScalar();
            conn.Close();
            return request.RequestID;
        }

        /*public int EditRequest(Request request)
        {
            SqlCommand cmd = new SqlCommand
            ("UPDATE Request SET Description = @description, Title = @title, AvailabilityFrom = @availabilityfrom, Hours = @hours, MaxCap = @maxcap, PointsEarned = @pointsearned, Status = @status, LocationID = @locationid WHERE RequestID = @selectedrequestID", conn);
            cmd.Parameters.AddWithValue("@selectedrequestID", request.RequestID);
            cmd.Parameters.AddWithValue("@description", request.Description);
            cmd.Parameters.AddWithValue("@title", request.Title);
            cmd.Parameters.AddWithValue("@availabilityfrom", request.AvailabilityFrom);
            cmd.Parameters.AddWithValue("@hours", request.Hours);
            cmd.Parameters.AddWithValue("@maxcap", request.MaxCap);
            cmd.Parameters.AddWithValue("@pointsearned", request.PointsEarned);
            cmd.Parameters.AddWithValue("@status", request.Status);
            cmd.Parameters.AddWithValue("@locationid", request.LocationID);

            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();

            return count;
        }
        */

        //deletes record from database
        public int DeleteStudentRequest(int studentid, int requestid)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM StudentRequest " +
                "WHERE RequestID = @selectedrequestid" + " AND StudentID = @selectedstudentid", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            cmd.Parameters.AddWithValue("@selectedrequestid", requestid);
            conn.Open();
            int rowCount;
            rowCount = cmd.ExecuteNonQuery();
            conn.Close();
            return rowCount;
        }

        //deletes record from database
        public int DeleteRequest(int requestID)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM Request " +
                "WHERE RequestID = @selectedrequestid", conn);
            cmd.Parameters.AddWithValue("@selectedrequestid", requestID);
            conn.Open();
            int rowCount;
            rowCount = cmd.ExecuteNonQuery();
            conn.Close();
            return rowCount;
        }
       
        public List<Request> GetAllRequests()
        {
            SqlCommand cmd = new SqlCommand(
             "SELECT * FROM Request ORDER BY RequestID DESC", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Request");
            conn.Close();
            List<Request> requestList = new List<Request>();
            foreach (DataRow row in result.Tables["Request"].Rows)
            {
                requestList.Add(
                    new Request
                    {
                        RequestID = Convert.ToInt32(row["RequestID"]),
                        DateRequest = Convert.ToDateTime(row["DateRequest"]),
                        Description = row["Description"].ToString(),
                        Title = row["Title"].ToString(),
                        AvailabilityFrom = Convert.ToDateTime(row["AvailabilityFrom"]),
                        Hours = Convert.ToInt32(row["Hours"]),
                        PointsEarned = Convert.ToInt32(row["PointsEarned"]),
                        Status = Convert.ToChar(row["Status"]),
                        LocationID = Convert.ToInt32(row["LocationID"]),
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        CategoryID = Convert.ToInt32(row["CategoryID"])
                    }
                );
            }
            return requestList;
        }

        public List<Request> GetMyRequests(int? studentid)
        {
            SqlCommand cmd = new SqlCommand(
             "SELECT * FROM Request WHERE StudentID = @selectedstudentid", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Request");
            conn.Close();
            List<Request> requestList = new List<Request>();
            foreach (DataRow row in result.Tables["Request"].Rows)
            {
                requestList.Add(
                    new Request
                    {
                        RequestID = Convert.ToInt32(row["RequestID"]),
                        DateRequest = Convert.ToDateTime(row["DateRequest"]),
                        Description = row["Description"].ToString(),
                        Title = row["Title"].ToString(),
                        AvailabilityFrom = Convert.ToDateTime(row["AvailabilityFrom"]),
                        Hours = Convert.ToInt32(row["Hours"]),
                        PointsEarned = Convert.ToInt32(row["PointsEarned"]),
                        Status = Convert.ToChar(row["Status"]),
                        LocationID = Convert.ToInt32(row["LocationID"]),
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        CategoryID = Convert.ToInt32(row["CategoryID"])
                    }
                );
            }
            return requestList;
        }

        public Request GetRequestByID(int requestid)
        {
            SqlCommand cmd = new SqlCommand(
             "SELECT * FROM Request WHERE RequestID = @selectedrequestid", conn);
            cmd.Parameters.AddWithValue("@selectedrequestid", requestid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "RequestDetails");
            conn.Close();

            Request request = new Request();
            if (result.Tables["RequestDetails"].Rows.Count > 0)
            {
                request.RequestID = requestid;

                DataTable table = result.Tables["RequestDetails"];

                if (!DBNull.Value.Equals(table.Rows[0]["RequestID"]))
                    request.RequestID = Convert.ToInt32(table.Rows[0]["RequestID"]);

                if (!DBNull.Value.Equals(table.Rows[0]["DateRequest"]))
                    request.DateRequest = Convert.ToDateTime(table.Rows[0]["DateRequest"]);

                if (!DBNull.Value.Equals(table.Rows[0]["Description"]))
                    request.Description = table.Rows[0]["Description"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["Title"]))
                    request.Title = table.Rows[0]["Title"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["AvailabilityFrom"]))
                    request.AvailabilityFrom = Convert.ToDateTime(table.Rows[0]["AvailabilityFrom"]);

                if (!DBNull.Value.Equals(table.Rows[0]["Hours"]))
                    request.Hours = Convert.ToInt32(table.Rows[0]["Hours"]);

                if (!DBNull.Value.Equals(table.Rows[0]["PointsEarned"]))
                    request.PointsEarned = Convert.ToInt32(table.Rows[0]["PointsEarned"]);

                if (!DBNull.Value.Equals(table.Rows[0]["Status"]))
                    request.Status = Convert.ToChar(table.Rows[0]["Status"]);

                if (!DBNull.Value.Equals(table.Rows[0]["LocationID"]))
                    request.LocationID = Convert.ToInt32(table.Rows[0]["LocationID"]);

                if (!DBNull.Value.Equals(table.Rows[0]["StudentID"]))
                    request.StudentID = Convert.ToInt32(table.Rows[0]["StudentID"]);

                if (!DBNull.Value.Equals(table.Rows[0]["CategoryID"]))
                    request.CategoryID = Convert.ToInt32(table.Rows[0]["CategoryID"]);

                return request;
            }

            else
            {
                return null; // Record not found
            }
        }
        public List<JoinedRequests> GetMyJoinedRequests(int studentid)
        {
            SqlCommand cmd = new SqlCommand(
             "SELECT r.RequestID, r.DateRequest, r.Description, r.Title, r.AvailabilityFrom, r.Hours, r.PointsEarned, l.LocationName, c.CategoryName" +
             " FROM Request r" + " INNER JOIN StudentRequest sr on r.RequestID = sr.RequestID" + " INNER JOIN Location l on r.LocationID = l.LocationID" + " INNER JOIN Category c on r.CategoryID = c.CategoryID"
             + " WHERE sr.StudentID = @selectedstudentid", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Results");
            conn.Close();
            List<JoinedRequests> joinedrequestsList = new List<JoinedRequests>();
            foreach (DataRow row in result.Tables["Results"].Rows)
            {
                joinedrequestsList.Add(
                    new JoinedRequests
                    {
                        RequestID = Convert.ToInt32(row["RequestID"]),
                        DateRequest = Convert.ToDateTime(row["DateRequest"]),
                        Title = row["Title"].ToString(),
                        AvailabilityFrom = Convert.ToDateTime(row["AvailabilityFrom"]),
                        Hours = Convert.ToInt32(row["Hours"]),
                        CategoryName = row["CategoryName"].ToString(),
                        PointsEarned = Convert.ToInt32(row["PointsEarned"]),
                        LocationName = row["LocationName"].ToString(),
                    });
            }
            return joinedrequestsList;
        }

        public int GetNumberOfRequests(int studentid)
        {
            SqlCommand cmd = new SqlCommand(
           "SELECT COUNT(*) FROM Request WHERE StudentID = @selectedstudentid", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            conn.Open();
            int rowsAmount = (int)cmd.ExecuteScalar();
            conn.Close();
            return rowsAmount;
        }
    }
}
