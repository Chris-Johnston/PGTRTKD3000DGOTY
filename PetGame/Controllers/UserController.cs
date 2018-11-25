using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetGame.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetGame
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        /// <summary>
        ///     Gets information from the user who is currently signed in, from the authorization header.
        /// </summary>
        /// <returns></returns>
        // GET: api/<controller>/whoami
        [HttpGet("whoami"), Authorize]
        public ActionResult WhoAmI()
        {
            var currentUser = HttpContext.User;

            var useridclaim = currentUser.Claims.FirstOrDefault(x => x.Type == "jti").Value;
            var usernameclaim = currentUser.Identities.FirstOrDefault()?.Name ?? "error";

            // test with a GET to /api/user with bearer authorization header from the login endpoint

            //return Content($"Hello {currentUser.ToString()}");
            return Content($"Hello {useridclaim} {usernameclaim}");
        }

        [HttpGet("whoami2"), Authorize]
        public IActionResult WhoAmIRendered()
        {
            // don't rely on the claims, since we are using cookie authentication
            var user = GetUserFromContext(HttpContext.User);
            var token = Request.Cookies["auth_token"];

            var u = GetUserFromToken(token);

            return Ok($"Hello {user.Username} {user.UserId}");
        }

        // get only the username and user id that owns this token
        // return null if invalid or out of date
        private User GetUserFromToken(string token)
        {
            //TODO
            return new User();
        }

        private User GetUserFromContext(ClaimsPrincipal userClaims)
        {
            try
            {
                // get the userId from the claims of the user and convert it to a ulong type
                var userid = Convert.ToUInt64(userClaims.Identities.FirstOrDefault().Claims.FirstOrDefault(z => z.Type == ClaimTypes.NameIdentifier)?.Value ?? "-1");
                string username = userClaims.Identities.FirstOrDefault().Claims.FirstOrDefault(z => z.Type == ClaimTypes.Name)?.Value ?? "error";

                // return a new user for this, without the password stuff
                return new User()
                {
                    UserId = userid,
                    Username = username
                };
            }
            catch
            {
                // catch everything, fail silently
                return null;
            }
        }
    }
}
