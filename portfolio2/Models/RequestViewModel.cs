﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class RequestViewModel
    {
        public int RequestID { get; set; }

        [Display(Name = "Date requested")]
        public DateTime DateRequest { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Availability From")]
        public DateTime AvailabilityFrom { get; set; }

        [Display(Name = "Hours")]
        public int Hours { get; set; }

        public string Photo { get; set; }

        [Display(Name = "Current capacity")]
        public int CurrCap { get; set; }

        [Display(Name = "Points earned")]
        public int PointsEarned { get; set; }

        [Display(Name = "Status")]
        public char Status { get; set; }

        [Display(Name = "Location ID")]
        public int LocationID { get; set; }

        [Display(Name = "Location Name")]
        public string LocationName { get; set; }

        [Display(Name = "Student ID")]
        public int StudentID { get; set; }

        [Display(Name = "Student Name")]
        public string Name { get; set; }

        public int CategoryID { get; set; }

        [Display(Name = "Location Name")]
        public string CategoryName { get; set; }

    }
}
