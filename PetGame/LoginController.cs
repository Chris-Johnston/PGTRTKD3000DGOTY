using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PetGame.Models;
using PetGame.Util;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetGame
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        /// <summary>
        ///     Collects login data and generates login tokens for the user to use.
        /// </summary>
        /// <param name="value"></param>
        // POST api/<controller>
        [HttpPost]
        public ActionResult Post([FromForm]string username, [FromForm]string password)
        { 
            //TODO: Run username and password against regex validation rules
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("value", "Neither the Username or Password may be null.");

            //HACK: Need to actually set up the database so I can have a username and password
            // when this is done, get the user from the database with the requested username
            User user = new User()
            {
                UserId = 1,
                Username = "DEV"
            };
            // HACK: don't use the hardcoded password
            Cryptography.SetUserPassword(user, "test");

            // if password verified, create a new token for that user and return it for the client
            if (Cryptography.VerifyUserPassword(user, password))
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
