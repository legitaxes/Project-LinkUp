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
    public class LocationDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        public LocationDAL()
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

        public List<Location> GetAllLocations()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Location", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "LocationList");
            conn.Close();
            List<Location> locationList = new List<Location>();
            foreach (DataRow row in result.Tables["LocationList"].Rows)
            {
                string photo = "";
                if (!DBNull.Value.Equals(row["Photo"]))
                    photo = row["Photo"].ToString();
                else
                    photo = "stocklocation.jpg";
                locationList.Add(
                    new Location
                    {
                        LocationID = Convert.ToInt32(row["LocationID"]),
                        LocationName = row["LocationName"].ToString(),
                        Photo = photo
                    });
            }
            return locationList;
        }
    }
}
