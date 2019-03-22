using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forms.Api.Models
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdPassport { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
    }
}
