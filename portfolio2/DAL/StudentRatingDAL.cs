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
    public class StudentRatingDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        //Constructor
        public StudentRatingDAL()
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

        public List<StudentRating> GetAllStudentRatings()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM StudentRating ORDER BY RatingID DESC", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "StudentRatingList");
            conn.Close();
            List<StudentRating> studentratingList = new List<StudentRating>();
            foreach (DataRow row in result.Tables["StudentRatingList"].Rows)
            {
                studentratingList.Add(
                    new StudentRating
                    {
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        RatingID = Convert.ToInt32(row["RatingID"])
                    });
            }
            return studentratingList;
        }
    }
}
