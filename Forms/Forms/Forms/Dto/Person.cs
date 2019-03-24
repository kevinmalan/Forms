using System.IO;
using Xamarin.Forms;

namespace Forms.Dto
{
    public class Account
    {
        public string FirstName { get; set; }
        public string IdPassport { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string ProfileImageBase64 { get; set; }
    }
}