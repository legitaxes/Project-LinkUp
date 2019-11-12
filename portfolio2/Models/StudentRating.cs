using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class StudentRating
    {
        [Display(Name = "Student ID")]
        public int StudentID { get; set; }

        [Display(Name = "Rating ID")]
        public int RatingID { get; set; }
    }
}
