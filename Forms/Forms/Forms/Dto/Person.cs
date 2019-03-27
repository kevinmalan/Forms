using System;
using System.IO;
using Xamarin.Forms;

namespace Forms.Dto
{
    public class Account
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string IdPassport { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string ProfileImageBase64 { get; set; }
    }

    public class AccountDto
    {
        public string FirstName { get; set; }
        public string IdPassport { get; set; }
        public string DateOfBirth { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public ImageSource ProfileImage{ get; set; }
    }

}