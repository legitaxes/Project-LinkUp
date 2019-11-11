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
    public class CourseDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        public CourseDAL()
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

        public List<Course> getAllCourse()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Course", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "CourseDetails");
            conn.Close();
            List<Course> courseList = new List<Course>();
            foreach (DataRow row in result.Tables["CourseDetails"].Rows)
            {
                courseList.Add(
                    new Course
                    {
                        CourseID = Convert.ToInt32(row["CourseID"]),
                        CourseName = row["CourseName"].ToString()
                    });
            }
            return courseList;
        }
    }
}
