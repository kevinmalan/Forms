using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forms.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Forms.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [Route("index")]
        [HttpGet]
        public IActionResult Index()
        {
            var p1 = new Person
            {
                FirstName = "Monty",
                LastName = "Burns",
                IdPassport = "5012171234567",
                DateOfBirth = new DateTime(1950,12,17)
            };

            var p2 = new Person
            {
                FirstName = "Cunty",
                LastName = "Runds",
                IdPassport = "6012171234567",
                DateOfBirth = new DateTime(1960, 12, 17)
            };

            var pList = new List<Person> { p1, p2 };

            return Ok(pList);
        }

        [Route("register")]
        [HttpPost]
        public IActionResult Register([FromBody] Person person)
        {
            var imageBytes = Convert.FromBase64String(person.ProfileImageBase64);

            return Ok();
        }
    }
}