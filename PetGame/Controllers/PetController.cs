using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PetGame.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PetGame
{
    //TODO: This controller class is missing all Authorization logic, so that needs to be added

    [Route("api/[controller]")]
    public class PetController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Pet> Get()
        {
            // gets all of the pets
            // not sure if we should actually support this endpoint
            return new List<Pet>()
            {
                new Pet()
                {
                    Birthday = DateTime.Now,
                    Endurance = 50,
                    IsDead = false,
                    Name = "Bobby Tables",
                    PetId = 123,
                    Strength = 50,
                    UserId = 1
                },
                new Pet()
                {
                    Birthday = DateTime.Today,
                    Endurance = 10,
                    IsDead = true,
                    Name = "King Tables the Fourth",
                    PetId = 111,
                    Strength = 10,
                    UserId = 2
                }
            };
        }

        // GET api/<controller>/5
        /// <summary>
        ///     Gets a Pet by Id.
        /// </summary>
        /// <param name="id">The ID of the Pet to get from the database.</param>
        /// <returns> A pet of the given ID, or null if unspecified. </returns>
        [HttpGet("{id}")]
        public Pet Get(ulong id)
        {
            return new Pet() { Birthday = DateTime.Now, Endurance = 50, IsDead = false, Name = "Bobby Tables", PetId = 123, Strength = 50, UserId = id };
        }

        /// <summary>
        ///     Inserts a new Pet into the database.
        /// </summary>
        /// <param name="value"> The pet to add to the database. </param>
        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]Pet value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value), "The supplied Pet cannot be null.");

            //TODO Add the pet to the database.
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
        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(ulong id, [FromBody]Pet value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value), "The supplied Pet cannot be null.");

            // TODO: PetController PUT updates
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
