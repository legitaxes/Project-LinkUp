using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class StudentViewModel
    {
        [Display(Name = "Student ID")]
        public int StudentID { get; set; }

        //[Required(ErrorMessage = "Please Do not Leave This Field Blank!")]
        [StringLength(50, ErrorMessage = "Name Cannot Exceed 50 Characters!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Do not Leave This Field Blank!")]
        [RegularExpression("^([123])$", ErrorMessage = "Please enter a valid year between 1 and 3")]
        public int? Year { get; set; }

        [Display(Name = "Student Number")]
        [StringLength(50, ErrorMessage = "Student Number cannot exceed 50 characters")]
        public string StudentNumber { get; set; }

        [Display(Name = "Photo")]
        [StringLength(255, ErrorMessage = "File name Cannot Exceed 255 Characters!")]
        public string Photo { get; set; }

        [Required(ErrorMessage = "Please do not leave this field blank!")]
        [RegularExpression("^[689][0-9]{7}$", ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Phone Number")]
        public int? PhoneNo { get; set; }

        [StringLength(255, ErrorMessage = "Name Cannot Exceed 255 Characters!")]
        public string Interest { get; set; }

        [Display(Name = "External Link")]
        [StringLength(255, ErrorMessage = "Name Cannot Exceed 255 Characters!")]
        public string ExternalLink { get; set; }

        [Display(Name = "Description")]
        [StringLength(3000, ErrorMessage = "Name Cannot Exceed 3000 Characters!")]
        public string Description { get; set; }

        public int? TotalPoints { get; set; }

        public int? Points { get; set; }

        [Display(Name = "Course ID")]
        public int CourseID { get; set; }

        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Display(Name = "Average rating")]
        [DisplayFormat(DataFormatString = "{0:#0.0}")]
        public double Rating { get; set; }

        [Display(Name = "Total amount of ratings")]
        public double TotalRatings { get; set; }
    }
}
