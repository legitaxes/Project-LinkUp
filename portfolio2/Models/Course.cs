using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class Course
    {
        public int CourseID { get; set; }

        [Display(Name = "Course")]
        public string CourseName { get; set; }
    }
}
