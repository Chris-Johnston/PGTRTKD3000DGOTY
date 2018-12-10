using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetGame.Core;
using PetGame.Models;
using PetGame.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetGame
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly SqlManager sqlManager;
        private readonly LoginService login;
        private readonly PetService petService;

        public UserController(SqlManager sqlManager) : base()
        {
            this.sqlManager = sqlManager;
            login = new LoginService(this.sqlManager);
            petService = new PetService(this.sqlManager);
        }

        [HttpGet("whoami"), AllowAnonymous]
        public IActionResult GetWhoAmI()
        {
            // do not trust the claims of user id from cookies, instead
            // verify the token in the claims against the values in the database
            // and get the user that matches that token
            var u = login.GetUserFromContext(HttpContext.User);

            if (u == null)
            {
                return Ok("You are not logged in.");
            }
            else
            {
                return Ok($"Hello {u.Username} {u.UserId}");
            }
        }
        // HACK: Only for testing. DO NOT EXPOSE THIS PUBLICLY
        //[HttpGet("sms")]
        //public IActionResult GetSMS([FromServices] TwilioService sms)
        //{
        //    sms.SendMessage(null, "Well, hello there.");
        //    return Ok();
        //}

        // creates a new pet for the current user
        [HttpPost("Pet")]
        public IActionResult PostPet([FromForm, FromBody] CreatePetModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.PetName))
                return BadRequest();

            var u = login.GetUserFromContext(HttpContext.User);
            if (u == null)
                return BadRequest();

            // make a new pet
            try
            {
                var pet = new Pet(model.PetName, u.UserId) { PetImageId = model.PetImageId };
                try
                {
                    pet = petService.InsertPet(pet);
                    if (pet == null)
                    {
                        return BadRequest();
                    }
                    return Redirect($"/pet/{pet.PetId}");
                }
                catch (SqlException)
                {
                    return BadRequest();
                }
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Retrieves the UserId of the currently logged in user and
        /// gets a list containing the status of all of the user's pets
        /// </summary>
        /// <returns></returns>
        [HttpGet("status")]
        public IActionResult Get()
        {
            //get currently logged in user
            var u = login.GetUserFromContext(HttpContext.User);
            ulong id = 0;

            //if the user is not logged in, inform them
            if (u == null)
            {
                return Unauthorized();
            }
            //set the currently logged in user's id as id
            else
            {
                id = u.UserId;
            }

            //get the list of status of the User's pets
            var UserPets = petService.GetPetStatusList(id);

            //if the user has no pets, return 404
            if (UserPets == null)
            {
                return NotFound();
            }
            //else, return the user's pets in JSON form
            else
            {
                return Json(UserPets);
            }
        }
    }
}
