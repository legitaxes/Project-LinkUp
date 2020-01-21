using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class Notification
    {
        public int NotificationID { get; set; }
        [Display(Name = "Notification")]
        public string NotificationName { get; set; }
        public char Status { get; set; }
        [Display(Name = "Date Posted")]
        [DataType(DataType.DateTime)] //-default html5 calendarpicker
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime DatePosted { get; set; }
        public int OwnerID { get; set; }
        public int SessionID { get; set; }
        public int StudentID { get; set; }
    }
}
