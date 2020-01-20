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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace portfolio2.DAL
{
    public class NotificationDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        public NotificationDAL()
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

        public List<Notification> GetAllNotification(int? studentid) //gets all the notification of the student
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Notification WHERE StudentID = @selectedstudentid", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "NotificationDetails");
            conn.Close();
            List<Notification> notificationList = new List<Notification>();
            foreach (DataRow row in result.Tables["NotificationDetails"].Rows)
            {
                notificationList.Add(
                    new Notification
                    {
                        NotificationID = Convert.ToInt32(row["NotificationID"]),
                        NotificationName = row["NotificationName"].ToString(),
                        Status = Convert.ToChar(row["Status"]),
                        DatePosted = Convert.ToDateTime(row["DatePosted"]),
                        StudentID = Convert.ToInt32(row["StudentID"])
                    });
            }
            return notificationList;
        }
    }
}
