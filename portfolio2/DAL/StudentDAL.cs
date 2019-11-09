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

        //checks if student exists in the database.. return true / else return false
        public bool checkStudent(string studentnumber)
        {
            SqlCommand cmd = new SqlCommand(
             "SELECT * FROM Student WHERE StudentNo = @selectedstudentnumber", conn);
            cmd.Parameters.AddWithValue("@selectedstudentnumber", studentnumber);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "StudentDetails");
            conn.Close();
            StudentDetails student = new StudentDetails();
            foreach (DataRow row in result.Tables["StudentDetails"].Rows)
            {
                student.StudentNumber = row["StudentNo"].ToString().ToLower();
                if (student.StudentNumber == studentnumber)
                {
                    return true;
                }
            }
            return false;
        }

        public List<StudentDetails> GetAllStudent()
        {
            SqlCommand cmd = new SqlCommand(
             "SELECT * FROM Student ORDER BY Points DESC", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "StudentDetails");
            conn.Close();
            List<StudentDetails> studentList = new List<StudentDetails>();
            foreach (DataRow row in result.Tables["StudentDetails"].Rows)
            {
                int? points;
                if (!DBNull.Value.Equals(row["Points"]))
                    points = Convert.ToInt32(row["Points"]);
                else
                    points = null;
                studentList.Add(
                    new StudentDetails
                    {
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        Name = row["Name"].ToString(),
                        Year = Convert.ToInt32(row["Year"]),
                        StudentNumber = row["StudentNo"].ToString(),
                        Photo = row["Photo"].ToString(),
                        PhoneNo = Convert.ToInt32(row["PhoneNo"]),
                        //Password = row["Password"].ToString(),
                        ExternalLink = row["ExternalLink"].ToString(),
                        Description = row["Description"].ToString(),
                        Points = points
                    }
                );
            }
            return studentList;
        }

        //adds the student to the database if he does not exist in the database
        public int Add(StudentDetails student)
        {
            SqlCommand cmd = new SqlCommand
            ("INSERT INTO Student (Name, Year, StudentNo, Photo, PhoneNo, ExternalLink, Description, Points)" +
            " OUTPUT INSERTED.StudentID" +
            " VALUES(@name, @year, @studentno, @photo, @phoneno, @externallink, @description, @points)", conn);
            cmd.Parameters.AddWithValue("@name", student.Name);
            cmd.Parameters.AddWithValue("@year", student.Year);
            cmd.Parameters.AddWithValue("@studentno", student.StudentNumber);
            cmd.Parameters.AddWithValue("@photo", DBNull.Value);
            cmd.Parameters.AddWithValue("@phoneno", student.PhoneNo);
            cmd.Parameters.AddWithValue("@externallink", student.ExternalLink);
            cmd.Parameters.AddWithValue("@description", student.Description);
            cmd.Parameters.AddWithValue("@points", DBNull.Value);
            conn.Open();
            student.StudentID = (int)cmd.ExecuteScalar();
            conn.Close();
            return student.StudentID;
        }
    }
}
