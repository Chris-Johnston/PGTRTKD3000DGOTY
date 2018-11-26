﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetGame.Core;
using PetGame.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetGame
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly SqlManager sqlManager;
        private readonly LoginService login;

        public UserController(SqlManager sqlManager) : base()
        {
            this.sqlManager = sqlManager;
            login = new LoginService(this.sqlManager);
        }

        [HttpGet("whoami"), AllowAnonymous]
        public IActionResult GetWhoAmI()
        {
            // do not trust the claims of user id from cookies, instead
            // verify the token in the claims against the values in the database
            // and get the user that matches that token
            var u = login.GetUserFromContext(HttpContext.User);

            if (u == null)
            {
                return Ok("You are not logged in.");
            }
            else
            {
                return Ok($"Hello {u.Username} {u.UserId}");
            }
        }
    }
}
