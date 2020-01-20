using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class Review
    {
        [Display(Name = "Description")]
        [StringLength(3000, ErrorMessage = "Name Cannot Exceed 3000 Characters!")]
        public string Description { get; set; }

        [Display(Name = "Stars")]
        public int Stars { get; set; }

        [Display(Name = "Rating Date")]
        public DateTime RatingDate { get; set; }

        [Display(Name = "Rating Type")]
        public Char RatingType { get; set; }
    }
}
