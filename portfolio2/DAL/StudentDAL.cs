﻿using System;
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
        
        public StudentDetails GetStudentDetails(int? studentid) //gets other students profile details
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Student WHERE StudentID = @selectedstudentid", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "ProfileDetails");
            conn.Close();
            StudentDetails student = new StudentDetails();
            if (result.Tables["ProfileDetails"].Rows.Count > 0)
            {
                DataTable table = result.Tables["ProfileDetails"];
                if (!DBNull.Value.Equals(table.Rows[0]["StudentID"]))
                    student.StudentID = Convert.ToInt32(table.Rows[0]["StudentID"]);

                if (!DBNull.Value.Equals(table.Rows[0]["Name"]))
                    student.Name = table.Rows[0]["Name"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["Year"]))
                    student.Year = Convert.ToInt32(table.Rows[0]["Year"]);

                if (!DBNull.Value.Equals(table.Rows[0]["StudentNo"]))
                    student.StudentNumber = table.Rows[0]["StudentNo"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["Photo"]))
                    student.Photo = table.Rows[0]["Photo"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["PhoneNo"]))
                    student.PhoneNo = Convert.ToInt32(table.Rows[0]["PhoneNo"]);

                if (!DBNull.Value.Equals(table.Rows[0]["Interest"]))
                    student.Interest = table.Rows[0]["Interest"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["ExternalLink"]))
                    student.ExternalLink = table.Rows[0]["ExternalLink"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["Description"]))
                    student.Description = table.Rows[0]["Description"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["TotalPoints"]))
                    student.TotalPoints = Convert.ToInt32(table.Rows[0]["TotalPoints"]);

                if (!DBNull.Value.Equals(table.Rows[0]["Points"]))
                    student.Points = Convert.ToInt32(table.Rows[0]["Points"]);

                if (!DBNull.Value.Equals(table.Rows[0]["CourseID"]))
                    student.CourseID = Convert.ToInt32(table.Rows[0]["CourseID"]);

                return student;
            }
            else
            {
                return null;
            }
        }
        public StudentDetails GetStudentBasedOnSession(int? sessionid) //gets sesion owner details
        {
            SqlCommand cmd = new SqlCommand(
            "SELECT st.Name, st.StudentID, st.Year, st.StudentNo, st.Photo, st.PhoneNo, st.Interest, st.ExternalLink, st.Description, st.TotalPoints, st.Points, st.CourseID  FROM Session s INNER JOIN Student st on s.StudentID = st.StudentID WHERE s.SessionID = @selectedsessionid", conn);
            cmd.Parameters.AddWithValue("@selectedsessionid", sessionid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Details");
            conn.Close();
            StudentDetails student = new StudentDetails();
            if (result.Tables["Details"].Rows.Count > 0)
            {
                DataTable table = result.Tables["Details"];

                if (!DBNull.Value.Equals(table.Rows[0]["StudentID"]))
                    student.StudentID = Convert.ToInt32(table.Rows[0]["StudentID"]);

                if (!DBNull.Value.Equals(table.Rows[0]["Name"]))
                    student.Name = table.Rows[0]["Name"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["Year"]))
                    student.Year = Convert.ToInt32(table.Rows[0]["Year"]);

                if (!DBNull.Value.Equals(table.Rows[0]["StudentNo"]))
                    student.StudentNumber = table.Rows[0]["StudentNo"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["Photo"]))
                    student.Photo = table.Rows[0]["Photo"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["PhoneNo"]))
                    student.PhoneNo = Convert.ToInt32(table.Rows[0]["PhoneNo"]);

                if (!DBNull.Value.Equals(table.Rows[0]["Interest"]))
                    student.Interest = table.Rows[0]["Interest"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["ExternalLink"]))
                    student.ExternalLink = table.Rows[0]["ExternalLink"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["Description"]))
                    student.Description = table.Rows[0]["Description"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["TotalPoints"]))
                    student.TotalPoints = Convert.ToInt32(table.Rows[0]["TotalPoints"]);

                if (!DBNull.Value.Equals(table.Rows[0]["Points"]))
                    student.Points = Convert.ToInt32(table.Rows[0]["Points"]);

                if (!DBNull.Value.Equals(table.Rows[0]["CourseID"]))
                    student.CourseID = Convert.ToInt32(table.Rows[0]["CourseID"]);

                return student;
            }
            else
            {
                return null;
            }

        }
        //get the logged in student details -- view/update profile
        public StudentDetails GetStudentDetails(string studentnumber)
        {
            SqlCommand cmd = new SqlCommand(
            "SELECT * FROM Student WHERE StudentNo = @selectedstudentnumber", conn);
            cmd.Parameters.AddWithValue("@selectedstudentnumber", studentnumber);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "Details");
            conn.Close();
            StudentDetails student = new StudentDetails();
            if (result.Tables["Details"].Rows.Count > 0)
            {
                DataTable table = result.Tables["Details"];

                if (!DBNull.Value.Equals(table.Rows[0]["StudentID"]))
                    student.StudentID = Convert.ToInt32(table.Rows[0]["StudentID"]);

                if (!DBNull.Value.Equals(table.Rows[0]["Name"]))
                    student.Name = table.Rows[0]["Name"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["Year"]))
                    student.Year = Convert.ToInt32(table.Rows[0]["Year"]);

                if (!DBNull.Value.Equals(table.Rows[0]["StudentNo"]))
                    student.StudentNumber = table.Rows[0]["StudentNo"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["Photo"]))
                    student.Photo = table.Rows[0]["Photo"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["PhoneNo"]))
                    student.PhoneNo = Convert.ToInt32(table.Rows[0]["PhoneNo"]);

                if (!DBNull.Value.Equals(table.Rows[0]["Interest"]))
                    student.Interest = table.Rows[0]["Interest"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["ExternalLink"]))
                    student.ExternalLink = table.Rows[0]["ExternalLink"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["Description"]))
                    student.Description = table.Rows[0]["Description"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["TotalPoints"]))
                    student.TotalPoints = Convert.ToInt32(table.Rows[0]["TotalPoints"]);

                if (!DBNull.Value.Equals(table.Rows[0]["Points"]))
                    student.Points = Convert.ToInt32(table.Rows[0]["Points"]);

                if (!DBNull.Value.Equals(table.Rows[0]["CourseID"]))
                    student.CourseID = Convert.ToInt32(table.Rows[0]["CourseID"]);

                return student;
            }
            else
            {
                return null;
            }
        }

        //given the student id, add the number of points to the student
        public void UpdateStudentPoints(int studentid, int? points, int? totalpoints)
        {
            SqlCommand cmd = new SqlCommand("UPDATE Student SET Points = @selectedstudentpoints, TotalPoints = @totalpoints" +
            " WHERE StudentID = @selectedstudentid", conn);
            if (points == null)
            {
                points = 0;
            }
            cmd.Parameters.AddWithValue("@totalpoints", totalpoints);
            cmd.Parameters.AddWithValue("@selectedstudentpoints", points);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
        }

        public StudentPhoto GetPhotoDetails(int studentID)
        {
            SqlCommand cmd = new SqlCommand("SELECT Name, Photo, StudentID FROM Student WHERE StudentID = @selectedStudentID", conn);
            cmd.Parameters.AddWithValue("@selectedStudentID", studentID);
            //object “cmd” as parameter.
            SqlDataAdapter da = new SqlDataAdapter(cmd);


            //Create a DataSet object “result"
            DataSet studentresult = new DataSet();

            //Open a database connection.
            conn.Open();

            //Use DataAdapter to fetch data to a table "StaffDetails" in DataSet. 
            da.Fill(studentresult, "StudentDetails");

            //Close the database connection 
            conn.Close();
            StudentPhoto studentPhoto = new StudentPhoto();
            if (studentresult.Tables["StudentDetails"].Rows.Count > 0)
            {
                studentPhoto.StudentID = studentID;

                DataTable table = studentresult.Tables["StudentDetails"];

                if (!DBNull.Value.Equals(table.Rows[0]["Name"]))
                    studentPhoto.Name = table.Rows[0]["Name"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["Photo"]))
                    studentPhoto.Photo = table.Rows[0]["Photo"].ToString();

                return studentPhoto;
            }
            else
                return null;
        }

        public int UploadPhoto(StudentPhoto student)
        {
            SqlCommand cmd = new SqlCommand("UPDATE Student SET Photo=@photo" +
                " WHERE StudentID=@selectedStudentID", conn);
            if (student.Photo != null)
                cmd.Parameters.AddWithValue("@photo", student.Photo);
            //else
            //    cmd.Parameters.AddWithValue("@photo", DBNull.Value);
            cmd.Parameters.AddWithValue("@selectedStudentID", student.StudentID);
            conn.Open();

            int count = cmd.ExecuteNonQuery();

            conn.Close();

            return count;
        }

        //updates the student profile
        public int Update(StudentDetails student)
        {
            SqlCommand cmd = new SqlCommand
            ("UPDATE Student SET Year=@year, PhoneNo=@phoneno, Interest=@interest, ExternalLink=@externallink, Description=@description" +
            " WHERE StudentNo = @selectedstudentNo", conn);
            cmd.Parameters.AddWithValue("@year", student.Year);
            cmd.Parameters.AddWithValue("@phoneno", student.PhoneNo);
            if (student.Interest == null)
                cmd.Parameters.AddWithValue("@interest", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@interest", student.Interest);
            if (student.ExternalLink == null)
                cmd.Parameters.AddWithValue("@externallink", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@externallink", student.ExternalLink);
            if (student.Description == null)
                cmd.Parameters.AddWithValue("@description", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@description", student.Description);
            cmd.Parameters.AddWithValue("@selectedstudentno", student.StudentNumber);
            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }

        public List<StudentDetails> GetSearchedStudent(string searchedvalue)
        {
            searchedvalue = "%" + searchedvalue + "%";
            SqlCommand cmd = new SqlCommand(
            "SELECT * FROM Student WHERE Name LIKE @selectedname", conn);
            cmd.Parameters.AddWithValue("@selectedname", searchedvalue);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "StudentDetails");
            conn.Close();
            List<StudentDetails> studentList = new List<StudentDetails>();
            foreach (DataRow row in result.Tables["StudentDetails"].Rows)
            {
                int? points;
                if (!DBNull.Value.Equals(row["TotalPoints"]))
                    points = Convert.ToInt32(row["TotalPoints"]);
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
                        Interest = row["Interest"].ToString(),
                        //Password = row["Password"].ToString(),
                        ExternalLink = row["ExternalLink"].ToString(),
                        Description = row["Description"].ToString(),
                        TotalPoints = points,
                        CourseID = Convert.ToInt32(row["CourseID"])
                    }
                );
            }
            return studentList;
        }

        public List<StudentDetails> GetAllStudent()
        {
            SqlCommand cmd = new SqlCommand(
             "SELECT * FROM Student ORDER BY TotalPoints DESC", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "StudentDetails");
            conn.Close();
            List<StudentDetails> studentList = new List<StudentDetails>();
            foreach (DataRow row in result.Tables["StudentDetails"].Rows)
            {
                int? points;
                if (!DBNull.Value.Equals(row["TotalPoints"]))
                    points = Convert.ToInt32(row["TotalPoints"]);
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
                        Interest = row["Interest"].ToString(),
                        //Password = row["Password"].ToString(),
                        ExternalLink = row["ExternalLink"].ToString(),
                        Description = row["Description"].ToString(),
                        TotalPoints = points,
                        CourseID = Convert.ToInt32(row["CourseID"])
                    }
                );
            }
            return studentList;
        }


        //adds the student to the database if he does not exist in the database
        public int Add(StudentDetails student)
        {
            SqlCommand cmd = new SqlCommand
            ("INSERT INTO Student (Name, Year, StudentNo, Photo, PhoneNo, Interest, ExternalLink, Description, TotalPoints, Points, CourseID)" +
            " OUTPUT INSERTED.StudentID" +
            " VALUES(@name, @year, @studentno, @photo, @phoneno, @interest, @externallink, @description, @totalpoints, @points, @courseID)", conn);
            if (student.Interest == null)
            {
                cmd.Parameters.AddWithValue("@interest", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@interest", student.Interest);
            }
            if (student.ExternalLink == null)
            {
                cmd.Parameters.AddWithValue("@externallink", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@externallink", student.ExternalLink);
            }
            if (student.Description == null)
            {
                cmd.Parameters.AddWithValue("@description", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@description", student.Description);
            }
            cmd.Parameters.AddWithValue("@name", student.Name);
            cmd.Parameters.AddWithValue("@year", student.Year);
            cmd.Parameters.AddWithValue("@studentno", student.StudentNumber);
            cmd.Parameters.AddWithValue("@photo", DBNull.Value);
            cmd.Parameters.AddWithValue("@phoneno", student.PhoneNo);
            cmd.Parameters.AddWithValue("@totalpoints", 0);
            cmd.Parameters.AddWithValue("@points", 0);
            cmd.Parameters.AddWithValue("@courseID", student.CourseID);
            conn.Open();
            student.StudentID = (int)cmd.ExecuteScalar();
            conn.Close();
            return student.StudentID;
        }

        public double GetReviewScore(int studentid)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM StudentRating sr INNER JOIN Rating r on r.RatingID = sr.RatingID WHERE StudentID = @selectedstudentid", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "ReviewPoints");
            conn.Close();
            double reviewstar = 0;
            int counter = 0;
            foreach (DataRow row in result.Tables["ReviewPoints"].Rows)
            {
                reviewstar = reviewstar + Convert.ToInt32(row["Stars"]);
                counter++;
            }
            if (counter != 0)
            {
                reviewstar = reviewstar / counter;
            }
            return reviewstar;
        }

        public List<StudentDetails> GetLeaderboardPoints()
        {
            SqlCommand cmd = new SqlCommand("SELECT StudentID, Photo, Name, TotalPoints, Points, StudentNo FROM Student ORDER BY TotalPoints DESC", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "StudentPointsList");
            conn.Close();
            List<StudentDetails> studentpointsList = new List<StudentDetails>();
            int p = 0;
            foreach (DataRow row in result.Tables["StudentPointsList"].Rows)
            {
                if (row["TotalPoints"] == DBNull.Value)
                {
                     p = 0;
                }
                else
                {
                     p = Convert.ToInt32(row["TotalPoints"]);
                }
                studentpointsList.Add(
                     new StudentDetails
                     {
                         StudentID = Convert.ToInt32(row["StudentID"]),
                         Photo = row["Photo"].ToString(),
                         StudentNumber = row["StudentNo"].ToString(),
                         Name = row["Name"].ToString(),
                         TotalPoints = p

                    });
            }
            return studentpointsList;
        }
    }
}
