using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetGame.Core;
using PetGame.Models;
using PetGame.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetGame
{
    //TODO: This controller class is missing all Authorization logic, so that needs to be added

    [Route("api/[controller]")]
    public class PetController : Controller
    {
        private readonly SqlManager sqlManager;
        private readonly PetService petService;
        private readonly LoginService loginService;

        public PetController(SqlManager sqlManager)
        {
            this.sqlManager = sqlManager;
            this.petService = new PetService(this.sqlManager);
            this.loginService = new LoginService(this.sqlManager);
        }

        // GET api/pet to return all is invalid, because that would
        // result in a lot of data being returned

        // GET api/<controller>/5
        /// <summary>
        ///     Gets a Pet by Id.
        /// </summary>
        /// <param name="id">The ID of the Pet to get from the database.</param>
        /// <returns> A pet of the given ID, or null if unspecified. </returns>
        [HttpGet("{id}"), AllowAnonymous]
        public IActionResult Get(ulong id)
        {
            var pet = petService.GetPetById(id);
            if (pet == null)
                return NotFound();
            return Json(pet);
        }

        /// <summary>
        ///     Inserts a new Pet into the database.
        ///     This action requires authentication.
        /// </summary>
        /// <param name="value"> The pet to add to the database. </param>
        // POST api/<controller>
        [HttpPost]
        public IActionResult Post([FromBody]Pet value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value), "The supplied Pet cannot be null.");
            // check user
            var user = loginService.GetUserFromContext(HttpContext.User);
            if (user?.UserId == value.UserId)
            {
                return Json(petService.InsertPet(value));
            }
            // unauthorized
            return Unauthorized();
        }

        /// <summary>
        ///     Updates a pet in the database.
        ///     Requires authorization.
        /// </summary>
        /// <param name="id">
        ///     The ID of the pet to update.
        /// </param>
        /// <param name="value">
        ///     The values of the pet to update.
        /// </param>
        // PUT api/<controller>
        [HttpPut("{id}")]
        public IActionResult Put(ulong id, [FromBody]Pet value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value), "The supplied Pet cannot be null.");
            // don't necessarily care if the PetId inside value does not match
            // the id passed separately, since only the Id is going to be used
            var pet = petService.UpdatePet(id, value);
            if (pet == null) return Unauthorized();
            return Json(pet);
        }

        /// <summary>
        ///     Deletes a pet from the database.
        /// </summary>
        /// <param name="id"></param>
        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(ulong id)
        {
            var user = loginService.GetUserFromContext(HttpContext.User);

            if (user == null)
                return Unauthorized();

            if (petService.DeletePet(id, user.UserId))
            {
                // deleted ok
                return Ok();
            }
            // didn't delete Ok, either not found or wrong user
            return Unauthorized();
        }
    }
}
