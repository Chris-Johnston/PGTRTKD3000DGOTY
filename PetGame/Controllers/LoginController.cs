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
using PetGame.Services;
using PetGame.Util;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetGame
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly SqlManager sqlManager;
        private readonly NotificationService twilio;

        public LoginController(SqlManager sqlManager, NotificationService twilio) : base()
        {
            this.sqlManager = sqlManager;
            this.twilio = twilio;
        }
        private void SignIn(UserToken ut)
        {
            // if the user token is null, don't sign them in
            // and return without doing anything
            if (ut == null)
                return;

            var cp = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, $"{ut.UserId}"),
                new Claim(ClaimTypes.UserData, ut.Token)
            }));

            HttpContext.User = cp;
            HttpContext.SignInAsync("Cookies", cp);
        }

        /// <summary>
        ///     Collects login data and generates login tokens for the user
        ///     for authentication.
        ///     
        ///     Returns with a rendered page showing that the user is logged in.
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        [AllowAnonymous]
        public IActionResult PostLogin()
        {
            var svc = new LoginService(sqlManager);
            var ut = svc.GetUserToken(this as ControllerBase);
            if (ut == null)
                return Unauthorized();
            SignIn(ut);
            // return to landing page
            return Redirect("/");
        }

        /// <summary>
        ///     Registers a new user with the supplied credentials.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult PostRegister()
        {
            var svc = new LoginService(sqlManager);
            UserToken ut;
            try
            {
                ut = svc.RegisterNewUser(this as ControllerBase, twilio);
            }
            catch (InsecurePasswordException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            if (ut == null)
                // bad data
                return Unauthorized();
            SignIn(ut);
            // return to landing page
            return Redirect("/");
        }
    }
}
