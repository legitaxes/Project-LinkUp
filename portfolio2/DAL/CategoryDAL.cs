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
    public class CategoryDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        public CategoryDAL()
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
        public List<Category> GetAllCategory()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Category", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "CategoryList");
            conn.Close();
            List<Category> categoryList = new List<Category>();
            foreach (DataRow row in result.Tables["CategoryList"].Rows)
            {
                string photo = "";
                if (!DBNull.Value.Equals(row["CategoryPhoto"]))
                    photo = row["CategoryPhoto"].ToString();
                else
                    photo = "stockcategory.jpg";
                categoryList.Add(
                    new Category
                    {
                        CategoryID = Convert.ToInt32(row["CategoryID"]),
                        CategoryName = row["CategoryName"].ToString(),
                        Description = row["Description"].ToString(),
                        CategoryPhoto = row["CategoryPhoto"].ToString()
                    });
            }
            return categoryList;
        }
        public List<SelectListItem> GetCategoryList()
        {
            SqlCommand cmd = new SqlCommand("SELECT CategoryID, CategoryName FROM Category " +
            "ORDER BY CategoryID ASC", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet result = new DataSet();
            conn.Open();
            da.Fill(result, "CategoryNameList");
            conn.Close();

            List<SelectListItem> categoryList = new List<SelectListItem>();
            foreach (DataRow row in result.Tables["CategoryNameList"].Rows)
            {
                categoryList.Add(
                    new SelectListItem
                    {
                        Value = row["CategoryID"].ToString(),
                        Text = row["CategoryName"].ToString()
                    });
            }
            return categoryList;
        }
    }
}
