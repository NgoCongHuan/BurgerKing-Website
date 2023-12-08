using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BurgerKing.Models
{
    public class Contact
    {
        public string name { get; set; }

        public string email { get; set; }

        public string subject { get; set; }
        
        public string message { get; set; }
    }
}