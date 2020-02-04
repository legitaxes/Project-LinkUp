using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class StudentTransaction
    {
        public int StudentID { get; set; }

        public int ItemID { get; set; }
    }
}
