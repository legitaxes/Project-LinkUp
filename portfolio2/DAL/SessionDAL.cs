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
    public class SessionDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        public SessionDAL()
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

        public List<Session> GetAllSessions()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Session", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "SessionList");
            conn.Close();
            List<Session> sessionList = new List<Session>();
            foreach (DataRow row in result.Tables["SessionList"].Rows)
            {
                string photo = "";
                if (!DBNull.Value.Equals(row["Photo"]))
                    photo = row["Photo"].ToString();
                else
                    photo = "stocksession.jpg";
                sessionList.Add(
                    new Session
                    {
                        SessionID = Convert.ToInt32(row["SessionID"]),
                        SessionDate = Convert.ToDateTime(row["SessionDate"]),
                        Name = row["Name"].ToString(),
                        Description = row["Description"].ToString(),
                        Photo = photo,
                        Hours = Convert.ToInt32(row["Hours"]),
                        Participants = Convert.ToInt32(row["Participants"]),
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        LocationID = Convert.ToInt32(row["LocationID"]),
                        CategoryID = Convert.ToInt32(row["CategoryID"])
                    });
            }
            return sessionList;
        }
    }
}
