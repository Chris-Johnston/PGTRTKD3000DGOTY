using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        ///     Collects login data and generates login tokens for the user to use.
        /// </summary>
        /// <param name="value"></param>
        // POST api/<controller>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody]LoginModel data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "The login data may not be null.");

            //TODO: Run username and password against regex validation rules
            if (string.IsNullOrWhiteSpace(data.username))
                throw new ArgumentException("Neither the Username or Password may be null or whitespace.");
            if (string.IsNullOrWhiteSpace(data.password))
                throw new ArgumentException("Neither the Username or Password may be null or whitespace.");

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

                // make a new cookie
                Response.Cookies.Append("auth_token", $"Bearer {ut}" , new Microsoft.AspNetCore.Http.CookieOptions()
                {
                    // so that these cannot be exposed from JS
                    HttpOnly = true,
                    Expires = DateTime.Now.AddDays(14)
                    //TODO add the secure flag to CookieOptions
                });

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                };

                var userid = new ClaimsIdentity(claims, "auth_token");
                var pr = new ClaimsPrincipal(userid);
                HttpContext.SignInAsync("Cookies", pr).Wait();
                //HttpContext.Authentication.SignInAsync

                // return the user token
                return Json(new { token = ut });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
