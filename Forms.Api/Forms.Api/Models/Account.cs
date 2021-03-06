﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Forms.Api.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdPassport { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string ProfileImageBase64 { get; set; }
        public DateTime DateTimeStamp { get; set; }
    }
}
