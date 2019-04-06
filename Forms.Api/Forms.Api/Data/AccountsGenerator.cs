using Forms.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Forms.Api.Data
{
    public class AccountsGenerator
    {
        public class PersonIdentity
        {
            public string IdPassport { get; set; }
            public DateTime DateOfBirth { get; set; }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AccountDbContext(
                    serviceProvider.GetRequiredService<DbContextOptions<AccountDbContext>>()))
            {
                if (context.Accounts.Any())
                    return;

                bool initiated = false;
                for (int i = 0; i < 2; i++)
                {
                    var identity = GetGeneratedPersonIdentity();
                    Account account;

                    if (!initiated)
                    {
                        account = new Account
                        {
                            FirstName = "Frank",
                            LastName = "Fruitfly",
                            Address = "123 Palm Avenue, Lynnwood, Pretoria, 0015, South Africa",
                            DateOfBirth = identity.DateOfBirth,
                            IdPassport = identity.IdPassport,
                            ProfileImageBase64 = GetImageBase64String("doctor.png"),
                            DateTimeStamp = new DateTime(2000, 01, 01)
                        };
                    }
                    else
                    {
                        account = new Account
                        {
                            FirstName = "Sally",
                            LastName = "Shulerz",
                            Address = "58 Rickson Drive, Centurion, Pretoria, 0014, South Africa",
                            DateOfBirth = identity.DateOfBirth,
                            IdPassport = identity.IdPassport,
                            ProfileImageBase64 = GetImageBase64String("seduca.png"),
                            DateTimeStamp = new DateTime(2000, 01, 02)
                        };
                    }

                    context.Accounts.Add(account);

                    initiated = true;
                }

                context.SaveChanges();
            }
        }

        private static string GetImageBase64String(string fileName)
        {
            var path = $"{Environment.CurrentDirectory}/Content/{fileName}";

            byte[] imageArr = File.ReadAllBytes(path);

            return Convert.ToBase64String(imageArr);
        }

        public static PersonIdentity GetGeneratedPersonIdentity()
        {
            Task.Delay(1).Wait();

            var random = new Random();

            string year = random.Next(30, 100).ToString();
            string month = random.Next(1, 13).ToString();
            string day = random.Next(1, 32).ToString();
            string filler = random.Next(1111111, 9999999).ToString();

            Amend(ref year);
            Amend(ref month);
            Amend(ref day);

            var idNumber = year + month + day + filler;

            DateTime.TryParseExact($"{year}{month}{day}", "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);

            return new PersonIdentity
            {
                IdPassport = idNumber,
                DateOfBirth = dateOfBirth
            };

            void Amend(ref string val)
            {
                if (val.Length < 2)
                    val = $"0{val}";
            }
        }

    }
}