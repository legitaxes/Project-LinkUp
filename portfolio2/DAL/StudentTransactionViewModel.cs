using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.DAL
{
    public class StudentTransactionViewModel
    {
        public int StudentID { get; set; }

        public int ItemID { get; set; }

        public string Photo { get; set; }

        public string ItemName { get; set; }

        public int Cost { get; set; }
    }
}
