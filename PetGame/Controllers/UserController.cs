using System;
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

        public UserController(SqlManager sqlManager) : base()
        {
            this.sqlManager = sqlManager;
        }

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

        [HttpGet("whoami2"), AllowAnonymous]
        public IActionResult WhoAmIRendered()
        {
            // don't rely on the claims, since we are using cookie authentication
            //var user = GetUserFromContext(HttpContext.User);
            //var token = Request.Cookies["auth_token"];

            // do not trust the claims from cookies

            string z = null;
            if (HttpContext.User.HasClaim(x => x.Type == ClaimTypes.UserData))
            {
                var claim = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.UserData);
                z = claim.Value;
            }

            var u = GetUserFromToken(z);

            if (u == null)
            {
                return Ok("Who are you?");
            }
            else
            {
                return Ok($"Hello {u.Username} {u.UserId}");
            }
        }

        // get only the username and user id that owns this token
        // return null if invalid or out of date
        private User GetUserFromToken(string token)
        {
            // validation of the token
            if (string.IsNullOrWhiteSpace(token))
                return null;

            ulong userid = 0;
            using (var conn = sqlManager.EstablishDataConnection)
            {
                // 0 is error case
                
                ulong usertokenid = 0;

                var cmd = conn.CreateCommand();
                // get the user id and user token id from the UserToken token
                // where the token has been used in the last 3 days
                cmd.CommandText =
                    @"SELECT UserId, UserTokenId FROM UserToken WHERE Token = @Token AND GETDATE() <= DATEADD(day, 3, GETDATE());";
                // add the token parameter
                cmd.Parameters.AddWithValue("@Token", token);

                using (var reader = cmd.ExecuteReader())
                {
                    // read the results
                    while (reader.Read())
                    {
                        userid = (ulong)reader.GetInt64(0);
                        usertokenid = (ulong)reader.GetInt64(1);
                    }
                }

                // query returned no results, or invalid
                if (userid == 0 && usertokenid == 0)
                {
                    // return null for no results
                    return null;
                }

                // update the last used time
                var updatecmd = conn.CreateCommand();
                updatecmd.CommandText =
                    @"UPDATE UserToken SET LastUsed = GETDATE() WHERE UserTokenId = @UserTokenId;";
                updatecmd.Parameters.AddWithValue("@UserTokenId", usertokenid.ToString());
                updatecmd.ExecuteNonQuery();

                // todo delete all UserToken that are out of date
            }

            return GetUserById(userid);
        }

        private User GetUserById(ulong id)
        {
            // todo add more input validation to GetUserById
            if (id <= 0) return null;

            User ret = null;

            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"SELECT UserId, Username FROM [User] WHERE UserId = @UserId;";
                cmd.Parameters.AddWithValue("@UserId", id.ToString());

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        ret = new User();
                        ret.UserId = id;
                        ret.Username = r.GetString(1);
                    }
                }
            }
            return ret;
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
