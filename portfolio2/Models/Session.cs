﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class Session
    {
        public int SessionID { get; set; }

        [Display(Name = "Session Date")]
        [DataType(DataType.Date)] //-default html5 calendarpicker
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime SessionDate { get; set; }

        [Required(ErrorMessage = "Please Do not Leave This Field Blank!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Do not Leave This Field Blank!")]
        public string Description { get; set; }

        public string Photo { get; set; }

        [Required(ErrorMessage = "Please Do not Leave This Field Blank!")]
        public int Hours { get; set; }

        [Required(ErrorMessage = "Please Do not Leave This Field Blank!")]
        public int Participants { get; set; }
            
        public int StudentID { get; set; }

        public int LocationID { get; set; }

        public int CategoryID { get; set; }

    }
}