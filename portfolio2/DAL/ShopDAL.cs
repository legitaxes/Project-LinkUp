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
    public class ShopDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        //Constructor
        public ShopDAL()
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

        public List<Shop> GetAllStoreItems()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Shop", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "ShopItems");
            conn.Close();
            List<Shop> storeList = new List<Shop>();
            foreach (DataRow row in result.Tables["ShopItems"].Rows)
            {
                storeList.Add(
                    new Shop
                    {
                        ItemID = Convert.ToInt32(row["ItemID"]),
                        ItemName = row["ItemName"].ToString(),
                        ItemDescription = row["ItemDescription"].ToString(),
                        Photo = row["Photo"].ToString(),
                        Cost = Convert.ToInt32(row["Cost"]) 
                    });
            }
            return storeList;
        }

        public Shop GetItemByID(int? itemid)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Shop WHERE ItemID = @selecteditemid", conn);
            cmd.Parameters.AddWithValue("@selecteditemid", itemid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "ItemDetails");
            conn.Close();
            Shop item = new Shop();
            if (result.Tables["ItemDetails"].Rows.Count > 0)
            {
                DataTable table = result.Tables["ItemDetails"];
                if (!DBNull.Value.Equals(table.Rows[0]["ItemID"]))
                    item.ItemID = Convert.ToInt32(table.Rows[0]["ItemID"]);

                if (!DBNull.Value.Equals(table.Rows[0]["Photo"]))
                    item.Photo = table.Rows[0]["Photo"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["ItemName"]))
                    item.ItemName = table.Rows[0]["ItemName"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["ItemDescription"]))
                    item.ItemDescription = table.Rows[0]["ItemDescription"].ToString();

                if (!DBNull.Value.Equals(table.Rows[0]["Cost"]))
                    item.Cost = Convert.ToInt32(table.Rows[0]["Cost"]);

                return item;
            }
            else
            {
                return null;
            }
        }
        public int Purchase(int cost, int studentid, int points)
        {
            SqlCommand cmd = new SqlCommand
            ("UPDATE Student SET Points = @finalpoints WHERE StudentID = @selectedstudentID", conn);
            int finalpoints = points - cost; 
            cmd.Parameters.AddWithValue("@selectedstudentID", studentid);
            cmd.Parameters.AddWithValue("@finalpoints", finalpoints);
            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }

        public int AddTransaction(int studentid, int itemid)
        {
            SqlCommand cmd = new SqlCommand
            ("INSERT INTO StudentTransaction (StudentID, ItemID)" +
            " VALUES(@selectedstudentid, @selecteditemid)", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            cmd.Parameters.AddWithValue("@selecteditemid", itemid); 
            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }

        public List<StudentTransaction> GetStudentTransactions(int studentid)
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM StudentTransaction WHERE StudentID = @selectedstudentid", conn);
            cmd.Parameters.AddWithValue("@selectedstudentid", studentid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "TransactionDetails");
            conn.Close();
            List<StudentTransaction> transactionList = new List<StudentTransaction>();

            foreach (DataRow row in result.Tables["TransactionDetails"].Rows)
            {
                transactionList.Add(
                    new StudentTransaction
                    {
                        StudentID = Convert.ToInt32(row["StudentID"]),
                        ItemID = Convert.ToInt32(row["ItemID"])
                    });
            }
            return transactionList;
        }
    }
}

