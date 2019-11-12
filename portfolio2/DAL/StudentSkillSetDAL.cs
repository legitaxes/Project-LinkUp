using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using portfolio2.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace portfolio2.DAL
{
    public class StudentSkillSetDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        public StudentSkillSetDAL()
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

        public List<StudentSkillSetViewModel> GetAllSkillSets()
        {
            //create a Sqlcommand object, with a SQL statement to retrieve 
            // all branch records, and a connection object to open
            //the database
            SqlCommand cmd = new SqlCommand("SELECT * FROM SkillSet", conn);
            //instantiate a dataAdapter object "da" and pass the Sqlcommand object cmd as parameter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            //create a DataSet object "result" to contain the records 
            //retrieved from database 
            DataSet result = new DataSet();
            //open database connection
            conn.Open();
            //Use DataAdapter "da" to fill up a DataTable "Skillset" in DataSet "result"
            da.Fill(result, "SkillSetDetails");
            //Close database connection
            conn.Close();
            //Transferring rows of data in DataTable to "Skillset" objects 
            List<StudentSkillSetViewModel> Skillsetlist = new List<StudentSkillSetViewModel>();
            foreach (DataRow row in result.Tables["SkillSetDetails"].Rows)
            {
                Skillsetlist.Add(new StudentSkillSetViewModel
                {
                    SkillSetID = Convert.ToInt32(row["SkillSetID"]),
                    SkillSetName = row["SkillSetName"].ToString()
                }
              );
            }
            return Skillsetlist;
        }

        public bool CheckStudentSkillSets(int skillsetID, int StudentID)
        {
            SqlCommand cmd = new SqlCommand
            ("SELECT SkillsetID FROM StudentSkillset WHERE StudentID = @selectedStudentID AND SkillsetID=@selectedSkillSetID", conn);
            cmd.Parameters.AddWithValue("@selectedStudentID", StudentID);
            cmd.Parameters.AddWithValue("@selectedSkillSetID", skillsetID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            //Use DataAdapter to fetch data to a table "EmailDetails" in DataSet.
            da.Fill(result, "SkillSetDetails");
            conn.Close();
            if (result.Tables["SkillSetDetails"].Rows.Count > 0)
                return true; //The email exists for another staff
            else
                return false; // The email address given does not exist
        }

        public int DeleteSkillSets(int StudentID)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM StudentSkillset where StudentID = @selectedStudentID", conn);
            cmd.Parameters.AddWithValue("@selectedStudentID", StudentID);
            int rowCount = 0;
            conn.Open();
            rowCount = cmd.ExecuteNonQuery();
            conn.Close();
            return rowCount;
        }

        public int UpdateSkillSets(int StudentID, int SkillsetID)
        {
            //Create a SqlCommand object, with a SQL statement to delete
            //all skillset records, and a connection object to open
            //the database.
            SqlCommand cmd = new SqlCommand("INSERT INTO StudentSkillSet(StudentID,SkillSetID) VALUES(@selectedStudentID,@SkillsetID)", conn);
            cmd.Parameters.AddWithValue("@selectedStudentID", StudentID);
            cmd.Parameters.AddWithValue("@SkillsetID", SkillsetID);
            //Open a database connection
            conn.Open();
            int rowCount = 0;
            //Execute the DELETE SQL to remove the staff record.
            rowCount = cmd.ExecuteNonQuery();
            //Close database connection.
            conn.Close();
            return rowCount;
        }
    }
}
