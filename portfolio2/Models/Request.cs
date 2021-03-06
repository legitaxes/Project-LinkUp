﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class Request
    {
        public int RequestID { get; set; }

        [Display(Name = "Date requested")]
        public DateTime DateRequest { get; set; }

        [Required(ErrorMessage = "Please Do not Leave This Field Blank!")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please Do not Leave This Field Blank!")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Availability From")]
        public DateTime AvailabilityFrom { get; set; }

        [Display(Name = "Hours")]
        public int Hours { get; set; }

        public string Photo { get; set; }

        [Display(Name = "Points earned")]
        public int PointsEarned { get; set; }

        [Display(Name = "Status")]
        public char Status { get; set; }

        [Display(Name = "Location ID")]
        public int LocationID { get; set; }

        [Display(Name = "StudentID")]
        public int StudentID { get; set; }

        public int CategoryID { get; set; }
    }
}
