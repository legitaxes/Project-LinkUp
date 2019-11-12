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
                        SessionID = Convert.ToInt32(row["SessionID"])
                    });
            }
            return ratingList;
        }
    }
}
