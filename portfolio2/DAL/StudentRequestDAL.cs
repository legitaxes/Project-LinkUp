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
    }
}
