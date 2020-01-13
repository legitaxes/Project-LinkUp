﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class JoinedRequests
    {
        [Display(Name = "Date requested")]
        public DateTime DateRequest { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Availability From")]
        public DateTime AvailabilityFrom { get; set; }

        [Display(Name = "Hours")]
        public int Hours { get; set; }

        [Display(Name = "Maximum capacity")]
        public int MaxCap { get; set; }

        [Display(Name = "Points earned")]
        public int PointsEarned { get; set; }

        [Display(Name = "Location Name")]
        public string LocationName { get; set; }
    }
}
