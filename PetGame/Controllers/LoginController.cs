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
        private void SignIn(UserToken ut)
        {
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
            SignIn(ut);
            
            if (ut == null)
                return Unauthorized();

            return RedirectToAction("GetWhoAmI", "User");
        }

        /// <summary>
        ///     Registers a new user with the supplied credentials.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult PostRegister()
        {
            var svc = new LoginService(sqlManager);
            var ut = svc.RegisterNewUser(this as ControllerBase);
            SignIn(ut);

            if (ut == null)
                // bad data
                return Unauthorized();

            return RedirectToAction("GetWhoAmI", "User");
        }
    }
}
