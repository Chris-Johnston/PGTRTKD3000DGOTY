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

        public PetController(SqlManager sqlManager)
        {
            this.sqlManager = sqlManager;
            this.petService = new PetService(this.sqlManager);
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
        public Pet Get(ulong id)
        {
            return petService.GetPetById(id);
        }

        /// <summary>
        ///     Inserts a new Pet into the database.
        ///     This action requires authentication.
        /// </summary>
        /// <param name="value"> The pet to add to the database. </param>
        // POST api/<controller>
        [HttpPost, Authorize]
        public Pet Post([FromBody]Pet value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value), "The supplied Pet cannot be null.");
            return petService.InsertPet(value);
        }

        /// <summary>
        ///     Updates a pet in the database.
        /// </summary>
        /// <param name="id">
        ///     The ID of the pet to update.
        /// </param>
        /// <param name="value">
        ///     The values of the pet to update.
        /// </param>
        // PUT api/<controller>
        [HttpPut("{id}")]
        public Pet Put(ulong id, [FromBody]Pet value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value), "The supplied Pet cannot be null.");
            // don't necessarily care if the PetId inside value does not match
            // the id passed separately, since only the Id is going to be used
            
        }

        /// <summary>
        ///     Deletes a pet from the database.
        /// </summary>
        /// <param name="id"></param>
        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(ulong id)
        {
            //TODO: Delete this pet from the database.
        }
    }
}
