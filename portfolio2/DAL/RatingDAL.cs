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
    public class RatingDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        //Constructor
        public RatingDAL()
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

        public List<Rating> GetAllRatings()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Rating", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "RatingList");
            conn.Close();
            List<Rating> ratingList = new List<Rating>();
            foreach (DataRow row in result.Tables["RatingList"].Rows)
            {
                ratingList.Add(
                    new Rating
                    {
                        RatingID = Convert.ToInt32(row["RatingID"]),
                        Description = row["Description"].ToString(),
                        Stars = Convert.ToInt32(row["Stars"]),
                        RatingDate = Convert.ToDateTime(row["RatingDate"]),
                        RatingType = Convert.ToChar(row["RatingType"]),
                        SessionID = Convert.ToInt32(row["SessionID"])
                    });
            }
            return ratingList;
        }

        public int GiveReviewToParticipant(Rating review) //pass in the review to give to insert into the database, insert into rating
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Rating (Description, Stars, RatingType, SessionID) " +
                "OUTPUT INSERTED.RatingID " +
                "VALUES(@desc, @stars, @ratingtype, @sessionid)", conn);
            cmd.Parameters.AddWithValue("@desc", review.Description);
            cmd.Parameters.AddWithValue("@stars", review.Stars);
            cmd.Parameters.AddWithValue("@ratingtype", 'P');
            cmd.Parameters.AddWithValue("@sessionid", review.SessionID);
            conn.Open();
            review.RatingID = (int)cmd.ExecuteScalar();
            conn.Close();
            return review.RatingID;
        }

        public void UpdateStudentRating(int studentid, int ratingid) //links ratingid to the student
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO StudentRating (StudentID, RatingID) " +
                " VALUES(@studentid, @ratingid)", conn);
            cmd.Parameters.AddWithValue("@studentid", studentid);
            cmd.Parameters.AddWithValue("@ratingid", ratingid);
            conn.Open();
            cmd.ExecuteScalar();
            conn.Close();
        }
    }
}
