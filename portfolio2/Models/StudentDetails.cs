using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class StudentDetails
    {
        [Display(Name = "Student ID")]
        public int StudentID { get; set; }

        //[Required(ErrorMessage = "Please Do not Leave This Field Blank!")]
        [StringLength(50, ErrorMessage = "Name Cannot Exceed 50 Characters!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Do not Leave This Field Blank!")]
        [RegularExpression("^([123])$", ErrorMessage = "Please enter a valid year between 1 and 3")]
        public int? Year { get; set; }

        [Display(Name="Student Number")]
        [StringLength(50, ErrorMessage ="Student Number cannot exceed 50 characters")]
        public string StudentNumber { get; set; }

        [Display(Name = "Photo")]
        [StringLength(255, ErrorMessage = "File name Cannot Exceed 255 Characters!")]
        public string Photo { get; set; }

        [Required(ErrorMessage = "Please do not leave this field blank!")]
        [RegularExpression("^[689][0-9]{7}$", ErrorMessage = "Please enter a 8 digit phone number")]
        [Display(Name = "Phone Number")]
        public int? PhoneNo { get; set; }

        [StringLength(255, ErrorMessage = "Interest Cannot Exceed 255 Characters!")]
        public string Interest { get; set; }

        [Display(Name = "External Link")]
        [StringLength(255, ErrorMessage = "External Link Cannot Exceed 255 Characters!")]
        public string ExternalLink { get; set; }

        [Display(Name = "Description")]
        [StringLength(3000, ErrorMessage = "Description Cannot Exceed 3000 Characters!")]
        public string Description { get; set; }

        [Display(Name = "Total Points")]
        public int? TotalPoints { get; set; }

        public int? Points { get; set; }
        
        [Display(Name="Course Name")]
        public int CourseID { get; set; }

        public double TotalReviewScore { get; set; }
    }
}
