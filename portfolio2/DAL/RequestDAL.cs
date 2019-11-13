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
    public class RequestDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;

        public RequestDAL()
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

        /*public void AddRequest()
        {

            SqlCommand cmd = new SqlCommand
            ("INSERT INTO Request (Name, Year, StudentNo, Photo, PhoneNo, Interest, ExternalLink, Description, Points, CourseID)" +
            " OUTPUT INSERTED.StudentID" +
            " VALUES(@name, @year, @studentno, @photo, @phoneno, @interest, @externallink, @description, @points, @courseID)", conn);
            cmd.Parameters.AddWithValue("@name", student.Name);
            cmd.Parameters.AddWithValue("@year", student.Year);
            cmd.Parameters.AddWithValue("@studentno", student.StudentNumber);
            cmd.Parameters.AddWithValue("@photo", DBNull.Value);
            cmd.Parameters.AddWithValue("@phoneno", student.PhoneNo);

        }*/
    }
}
