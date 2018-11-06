﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PetGame.Core;
using PetGame.Models;
using PetGame.Util;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetGame
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly SqlManager sqlManager;

        public LoginController(SqlManager sqlManager) : base()
        {
            this.sqlManager = sqlManager;
        }

        public class LoginData
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        /// <summary>
        ///     Collects login data and generates login tokens for the user to use.
        /// </summary>
        /// <param name="value"></param>
        // POST api/<controller>
        [HttpPost]
        public ActionResult Post([FromBody]LoginData data)
        {
            //TODO: Run username and password against regex validation rules
            //if (string.IsNullOrWhiteSpace(username))
            //    throw new ArgumentNullException(nameof(username), "Neither the Username or Password may be null.");
            //if (string.IsNullOrWhiteSpace(password))
            //    throw new ArgumentNullException(nameof(password), "Neither the Username or Password may be null.");

            //HACK: Need to actually set up the database so I can have a username and password
            // when this is done, get the user from the database with the requested username
            User user = new User()
            {
                UserId = 1,
                Username = data.username
            };
            // HACK: don't use the hardcoded password
            Cryptography.SetUserPassword(user, "test");

            //TODO: TEST --- remove me when done
            // example of how we should do SQL stuff without use of EF
            using (var s = sqlManager.EstablishDataConnection)
            {
                var cmd = s.CreateCommand();
                cmd.CommandText = "INSERT INTO [User] (UserId, Username, PasswordHash, HMACKey) VALUES (@UserId, @Username, @PasswordHash, @HMACKey);";
                cmd.Parameters.AddWithValue("@UserId", 123);
                cmd.Parameters.AddWithValue("@Username", "test person'");
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@HMACKey", user.HMACKey);

                int a = cmd.ExecuteNonQuery();
                Console.WriteLine($"Affected {a}");
            }

            // if password verified, create a new token for that user and return it for the client
            if (Cryptography.VerifyUserPassword(user, data.password))
            {
                // get a user token for this suer
                var ut = Cryptography.MakeUserToken(user);
                Response.StatusCode = 200;
                // return the user token
                return Json(ut);
            }
            else
            {
                Response.StatusCode = 403;
                return Content("Invalid credentials.");
            }
        }
    }
}
