using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class Location
    {
        public int LocationID { get; set; }

        [Display(Name = "Location Name")]
        public string LocationName { get; set; }

        public string Photo { get; set; }
    }
}
