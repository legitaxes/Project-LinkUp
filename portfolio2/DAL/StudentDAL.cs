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
    public class StudentDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        //Constructor
        public StudentDAL()
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
        public bool checkStudent(string name)
        {
            SqlCommand cmd = new SqlCommand(
             "SELECT * FROM Student WHERE Name = @selectedname", conn);
            cmd.Parameters.AddWithValue("@selectedname", name);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "StudentDetails");
            conn.Close();
            Student student = new Student();
            foreach (DataRow row in result.Tables["StudentDetails"].Rows)
            {
                student.Name = row["Name"].ToString().ToLower();
                if (student.Name == name)
                {
                    string status = row["Status"].ToString();
                    if (status == "N")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        public List<Student> GetAllStudent()
        {
            SqlCommand cmd = new SqlCommand(
             "SELECT * FROM Student ORDER BY StudentID", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "StudentDetails");
            conn.Close();
            List<Student> studentList = new List<Student>();
            foreach (DataRow row in result.Tables["StudentDetails"].Rows)
            {
                int? points;
                if (!DBNull.Value.Equals(row["Points"]))
                    points = Convert.ToInt32(row["Points"]);
                else
                    points = null;
                studentList.Add(
                    new Student
                    {
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        Name = row["Name"].ToString(),
                        Year = Convert.ToInt32(row["Year"]),
                        EmailAddr = row["Email"].ToString(),
                        Photo = row["Photo"].ToString(),
                        PhoneNo = Convert.ToInt32(row["PhoneNo"]),
                        Password = row["Password"].ToString(),
                        ExternalLink = row["ExternalLink"].ToString(),
                        Description = row["Description"].ToString(),
                        Points = points
                    }
                );
            }
            return studentList;
        }
        public int Add(Student student)
        {
            SqlCommand cmd = new SqlCommand
            ("INSERT INTO Student (Name, Year, Email, @photo Description)" +
            " OUTPUT INSERTED.LecturerID" +
            " VALUES(@name, @email, @password, @description)", conn);
            return 0;
        }
    }
}
