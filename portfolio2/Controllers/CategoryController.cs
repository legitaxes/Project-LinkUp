using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using portfolio2.Models;
using portfolio2.DAL;

namespace portfolio2.Controllers
{
    public class CategoryController : Controller
    {
        private CategoryDAL categoryContext = new CategoryDAL();
        public IActionResult Index()
        {
            List<Category> categoryList = categoryContext.GetAllCategory();
            return View(categoryList);
        }
    }
}