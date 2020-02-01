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
            SqlCommand cmd = new SqlCommand("SELECT * FROM Notification WHERE StudentID = @selectedstudentid ORDER BY DatePosted DESC", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "NotificationDetails");
            conn.Close();
            List<Notification> notificationList = new List<Notification>();
            int sid = 0;
            foreach (DataRow row in result.Tables["NotificationDetails"].Rows)
            {
                if (row["SessionID"] != DBNull.Value)
                {
                    sid = Convert.ToInt32(row["SessionID"]);
                }
                notificationList.Add(
                    new Notification
                    {
                        NotificationID = Convert.ToInt32(row["NotificationID"]),
                        NotificationName = row["NotificationName"].ToString(),
                        Status = Convert.ToChar(row["Status"]),
                        DatePosted = Convert.ToDateTime(row["DatePosted"]),
                        OwnerID = Convert.ToInt32(row["OwnerID"]),
                        SessionID = sid,
                        StudentID = Convert.ToInt32(row["StudentID"])
                    });
            }
            return notificationList;
        }

        public int RemoveNotification(int notificationid)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM Notification " + 
                "WHERE NotificationID = @notificationid", conn);
            cmd.Parameters.AddWithValue("@notificationid", notificationid);
            conn.Open();
            int count;
            count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }

        public int AddReviewNotification(int studentid, int? ownerid, int? sessionid)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Notification (NotificationName, Status, OwnerID, SessionID, StudentID) " +
                "OUTPUT INSERTED.NotificationID " +
                "VALUES(@notiname, @status, @ownerid, @sessionid, @studentid)", conn);
            cmd.Parameters.AddWithValue("@notiname", "You have a review to give to the session owner that you have recently attended of!");
            cmd.Parameters.AddWithValue("@status", 'N');
            cmd.Parameters.AddWithValue("@ownerid", ownerid);
            cmd.Parameters.AddWithValue("@sessionid", sessionid);
            cmd.Parameters.AddWithValue("@studentid", studentid);
            conn.Open();
            int notificationid = (int)cmd.ExecuteScalar();
            conn.Close();
            return notificationid;
        }

        public int AddSessionCancelNotification(int studentid, int? ownerid, int? sessionid)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Notification (NotificationName, Status, OwnerID, SessionID, StudentID) " +
            "OUTPUT INSERTED.NotificationID " +
            "VALUES(@notiname, @status, @ownerid, @sessionid, @studentid)", conn);
            cmd.Parameters.AddWithValue("@notiname", "A session that you have signed up for is cancelled!");
            cmd.Parameters.AddWithValue("@status", 'N');
            cmd.Parameters.AddWithValue("@ownerid", ownerid);
            cmd.Parameters.AddWithValue("@sessionid", sessionid);
            cmd.Parameters.AddWithValue("@studentid", studentid);
            conn.Open();
            int notificationid = (int)cmd.ExecuteScalar();
            conn.Close();
            return notificationid;
        }

        public int AddJoinedRequestNotification(int ownerid, int sessionid, int studentid)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Notification (NotificationName, Status, OwnerID, SessionID, StudentID) " +
            "OUTPUT INSERTED.NotificationID " +
            "VALUES(@notiname, @status, @ownerid, @sessionid, @studentid)", conn);
            cmd.Parameters.AddWithValue("@notiname", "Someone has took up your request! Click the button to view the session details!");
            cmd.Parameters.AddWithValue("@status", 'N');
            cmd.Parameters.AddWithValue("@ownerid", ownerid);
            cmd.Parameters.AddWithValue("@sessionid", sessionid);
            cmd.Parameters.AddWithValue("@studentid", studentid);
            conn.Open();
            int notificationid = (int)cmd.ExecuteScalar();
            conn.Close();
            return notificationid;
        }

        public int UpdateNotificationStatus(int notificationid)
        {
            SqlCommand cmd = new SqlCommand("UPDATE Notification SET Status=@status" +
                " WHERE NotificationID = @selectednotificationid", conn);
            cmd.Parameters.AddWithValue("@status", 'Y');
            cmd.Parameters.AddWithValue("@selectednotificationid", notificationid);
            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }

        public int AddReviewGivenNotification(int studentid, int? ownerid)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Notification (NotificationName, Status, OwnerID, SessionID, StudentID) " +
            "OUTPUT INSERTED.NotificationID " +
            "VALUES(@notiname, @status, @ownerid, @sessionid, @studentid)", conn);
            cmd.Parameters.AddWithValue("@notiname", "Someone left you a review!");
            cmd.Parameters.AddWithValue("@status", 'N');
            cmd.Parameters.AddWithValue("@ownerid", ownerid);
            cmd.Parameters.AddWithValue("@sessionid", DBNull.Value);
            cmd.Parameters.AddWithValue("@studentid", studentid);
            conn.Open();
            int notificationid = (int)cmd.ExecuteScalar();
            conn.Close();
            return notificationid;
        }

    }
}
