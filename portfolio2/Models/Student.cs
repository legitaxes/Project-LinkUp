using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class Student
    {
        [Display(Name = "Student ID")]
        public int StudentID { get; set; }

        [Required(ErrorMessage = "Please Do not Leave This Field Blank!")]
        [StringLength(50, ErrorMessage = "Name Cannot Exceed 50 Characters!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Do not Leave This Field Blank!")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Please Do not Leave This Field Blank!")]
        [Display(Name = "NP Email Address")]
        [EmailAddress]
        //[ValidateEmailExists(ErrorMessage = "Email Address already exists!")]
        [StringLength(50, ErrorMessage = "Email Cannot Exceed 50 Characters!")]
        public string EmailAddr { get; set; }

        [Display(Name = "Photo")]
        [StringLength(255, ErrorMessage = "File name Cannot Exceed 255 Characters!")]
        public string Photo { get; set; }

        [Display(Name = "Phone Number")]
        public int PhoneNo { get; set; }

        [StringLength(255, ErrorMessage = "Password Length Cannot Exceed 255 Characters!")]
        public string Password { get; set; }

        [Display(Name = "External Link")]
        [StringLength(255, ErrorMessage = "Name Cannot Exceed 255 Characters!")]
        public string ExternalLink { get; set; }

        [Display(Name = "Description")]
        [StringLength(3000, ErrorMessage = "Name Cannot Exceed 3000 Characters!")]
        public string Description { get; set; }

        public int? Points { get; set; }

        //[Display(Name = "Course")]
        //[Required(ErrorMessage = "Please Do not Leave This Field Blank!")]
        //[StringLength(50, ErrorMessage = "Name Cannot Exceed 50 Characters!")]
        //public string Course { get; set; }






    }
}
