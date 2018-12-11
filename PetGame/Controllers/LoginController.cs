using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            try
            {
                var ut = svc.GetUserToken(this as ControllerBase);
                if (ut == null)
                {
                    return Redirect($"/Login?Error={ErrorBadCredentials}");
                    //return Unauthorized();
                }

                SignIn(ut);
                // return to landing page
                return Redirect("/");
            }
            catch (ArgumentException)
            {
                return Redirect($"/Login?Error={ErrorInvalidArguments}");
            }
        }

        public const string ErrorBadCredentials = "InvalidCredentials";
        public const string ErrorInsecurePassword = "InsecurePassword";
        public const string ErrorInvalidArguments = "InvalidArguments";
        public const string ErrorProbablyDuplicate = "Duplicate";

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
                // redirect back to itself
                return Redirect($"/Login?Error={ErrorInsecurePassword}");
            }
            catch (ArgumentException e)
            {
                return Redirect($"/Login?Error={ErrorInvalidArguments}");
            }
            catch (SqlException e)
            {
                // this could also be thrown if the sql server connection is dead
                // or if the connection string is invalid or something, but we are assuming not the case.
                return Redirect($"/Login?Error={ErrorProbablyDuplicate}");
            }
            if (ut == null)
                // bad data
                // not sure this would ever happen in this case.
                return Unauthorized();
            SignIn(ut);
            // return to landing page
            return Redirect("/");
        }
    }
}
