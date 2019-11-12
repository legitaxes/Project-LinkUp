using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        [Display(Name ="Name")]
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string CategoryPhoto { get; set; }
    }
}
