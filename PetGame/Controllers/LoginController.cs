using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        // POST api/<controller>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult PostLogin()
        {
            var svc = new LoginService(sqlManager);
            var ut = svc.GetUserToken(this as ControllerBase);

            if (ut == null)
                return Unauthorized();
            return Json(ut);
        }

        /// <summary>
        ///     Collects login data and generates login tokens for the user
        ///     for authentication.
        ///     
        ///     Returns with a rendered page showing that the user is logged in.
        /// </summary>
        /// <returns></returns>
        [HttpPost("whoami")]
        [AllowAnonymous]
        public IActionResult PostLoginWhoami()
        {
            var svc = new LoginService(sqlManager);

            // Response.Cookies.Delete("auth_token");
            var result = SignOut(CookieAuthenticationDefaults.AuthenticationScheme);

            var ut = svc.GetUserToken(this as ControllerBase);

            //var claims = new List<Claim>()
            //{
            //    // new Claim(ClaimTypes.Name, ut.Username),
            //    new Claim(ClaimTypes.NameIdentifier, ut.UserId.ToString())
            //};
            var cp = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, ut.UserId.ToString()),
                new Claim(ClaimTypes.UserData, ut.Token)
            }));

            HttpContext.User = cp;

            HttpContext.SignInAsync("Cookies", cp);

            //HttpContext.User.AddIdentity(new ClaimsIdentity("Cookie")
            //{

            //});



            //var r = HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme).Result;
            //result.Properties = new AuthenticationProperties();
            //result.Properties.UpdateTokenValue("auth_token", ut.Token);

            //HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, r.Principal, r.Properties);

            //var userid = new ClaimsIdentity(claims, "auth_token");
            //var pr = new ClaimsPrincipal(userid);

            

            //var signinresult = SignIn(pr, CookieAuthenticationDefaults.AuthenticationScheme);



            if (ut == null)
                return Unauthorized();

            return Ok($"Logged in as user id {ut.UserId}");
        }

        /// <summary>
        ///     Registers a new user with the supplied credentials.
        /// </summary>
        [HttpPost("register")]
        public IActionResult PostRegister()
        {
            var svc = new LoginService(sqlManager);
            var ut = svc.RegisterNewUser(this as ControllerBase);

            // Response.Cookies.Delete("auth_token");

            // sign out of the authentication scheme
            var result = SignOut(CookieAuthenticationDefaults.AuthenticationScheme);


            if (ut == null)
                // bad data
                return Unauthorized();

            return Ok($"Registered a new user with id {ut.UserId}");
        }
    }
}
