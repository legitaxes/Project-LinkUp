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
            ("INSERT INTO Request (DateRequest, Description, Title, AvailabilityFrom, AvailabilityTo, PointsEarned, Status, LocationID, StudentID)" +
            " OUTPUT INSERTED.RequestID" +
            " VALUES(@daterequest, @description, @title, @availabilityfrom, @availabilityto, @pointsearned, @status, @locationid, @studentid)", conn);
            cmd.Parameters.AddWithValue("@daterequest", request.DateRequest);
            cmd.Parameters.AddWithValue("@description", request.Description);
            cmd.Parameters.AddWithValue("@title", request.Title);
            cmd.Parameters.AddWithValue("@availabilityfrom", request.AvailabilityFrom);
            cmd.Parameters.AddWithValue("@availabilityto", request.AvailabilityTo);
            cmd.Parameters.AddWithValue("@pointsearned", request.PointsEarned);
            cmd.Parameters.AddWithValue("@status", request.Status);
            cmd.Parameters.AddWithValue("@locationid", request.LocationID);
            cmd.Parameters.AddWithValue("@studentid", request.StudentID);
            conn.Open();
            request.RequestID = (int)cmd.ExecuteScalar();
            conn.Close();
            return request.RequestID;
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
                        AvailabilityTo = Convert.ToDateTime(row["AvailabilityTo"]),
                        PointsEarned = Convert.ToInt32(row["PointsEarned"]),
                        Status = Convert.ToChar(row["Status"]),
                        LocationID = Convert.ToInt32(row["LocationID"]),
                        StudentID = Convert.ToInt32(row["StudentID"])
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
                        AvailabilityTo = Convert.ToDateTime(row["AvailabilityTo"]),
                        PointsEarned = Convert.ToInt32(row["PointsEarned"]),
                        Status = Convert.ToChar(row["Status"]),
                        LocationID = Convert.ToInt32(row["LocationID"]),
                        StudentID = Convert.ToInt32(row["StudentID"])
                    }
                );
            }
            return requestList;
        }
    }
}
