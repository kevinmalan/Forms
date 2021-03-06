﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forms.Api.Data;
using Forms.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Forms.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountDbContext _db;

        public AccountController(AccountDbContext db)
        {
            _db = db;
        }

        [Route("index")]
        [HttpGet]
        public IActionResult Index()
        {
            var accounts = _db.Accounts.ToList();

            return Ok(accounts);
        }

        [Route("count")]
        [HttpGet]
        public int GetAccountCount()
        {
            var count = _db.Accounts.Count();

            return count;
        }

        [Route("register")]
        [HttpPost]
        public IActionResult Register([FromBody] Account account)
        {
            _db.Accounts.Add(account);
            _db.SaveChanges();

            return Ok();
        }

        [Route("delete")]
        [HttpPost]
        public IActionResult Delete([FromBody] string idPassport)
        {
            var accountToDelete = _db.Accounts.First(a => a.IdPassport == idPassport);

            _db.Accounts.Remove(accountToDelete);
            _db.SaveChanges();

            return Ok();
        }

    }
}