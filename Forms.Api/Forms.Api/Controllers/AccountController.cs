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
            var p = new Person
            {
                FirstName = "Monty",
                LastName = "Burns",
                IdPassport = "5012171234567",
                DateOfBirth = new DateTime(1990,12,17)
            };

            return Ok(p);
        }

        [Route("register")]
        [HttpPost]
        public IActionResult Register([FromBody] Person person)
        {
            return Ok();
        }
    }
}