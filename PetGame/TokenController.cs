using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetGame.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody] LoginModel login)
        {
            var user = Authenticate(login);

            if (user != null)
            {
                return Ok(new { token = "aksdjflskf" });
            }

            return Unauthorized();
        }

        private void BuildToken()
        {
            // var token = new JwtSecurityToken(HttpContext.Request.Host)
        }

        private User Authenticate(LoginModel login)
        {
            //todo hack
            if (login.password == "test")
            {
                return new User()
                {
                    UserId = 123,
                    Username = "user"
                };
            }
            return null;
        }
    }
}
