using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio2.Models
{
    public class Account
    {
        public string Message { get; set; }
        public Accounts Accounts { get; set; }
    }
    public class Accounts
    {
        public string Name { get; set; }
    }
}
