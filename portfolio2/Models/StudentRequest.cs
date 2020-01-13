using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class StudentRequest
    {
        [Display(Name = "StudentID")]
        public int StudentID { get; set; }

        [Display(Name = "RequestID")]
        public int RequestID { get; set; }
    }
}
