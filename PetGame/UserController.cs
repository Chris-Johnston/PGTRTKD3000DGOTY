using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [HttpGet]
        public ActionResult Get()
        {
            // get the authorization headers
            var authorization = HttpContext.Request.Headers["Authorization"];

            // HACK: Add graceful handling for when the request isn't specified, and add a utility function to get the user from the current httpcontext

            //HACK: use the database to get the associated user id for a valid token that matches this authorization (if any)
            var user = new User()
            {
                Username = "TEST",
                UserId = 1
            };

            //return Json(user);
            return Content($"Hello {user.Username}");
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
