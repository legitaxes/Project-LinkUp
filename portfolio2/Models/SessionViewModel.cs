using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class SessionViewModel
    {
        public int SessionID { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.DateTime)] //-default html5 calendarpicker
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

        [Display(Name = "Finished")]
        public char Status { get; set; }

        [Display(Name = "Host")]
        public string StudentName { get; set; }

        [Display(Name = "Location")]
        public string LocationName { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }
    }
}
