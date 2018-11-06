using System;
using System.Collections.Generic;
using System.Linq;
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
        // GET: api/<controller>
        [HttpGet, Authorize]
        public ActionResult Get()
        {
            var currentUser = HttpContext.User;

            var useridclaim = currentUser.Claims.FirstOrDefault(x => x.Type == "jti").Value;
            var usernameclaim = currentUser.Identities.FirstOrDefault()?.Name ?? "error";


            //return Content($"Hello {currentUser.ToString()}");
            return Content($"Hello {useridclaim} {usernameclaim}");
        }
    }
}
