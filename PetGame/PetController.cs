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
    public class PetController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Pet> Get()
        {
            return new List<Pet>()
            {
                new Pet() { Birthday = DateTime.Now, Endurance = 50, IsDead = false, Name = "Bobby Tables", PetId = 123, Strength = 50, UserId = 1 }
            };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public Pet Get(ulong id)
        {
            return new Pet() { Birthday = DateTime.Now, Endurance = 50, IsDead = false, Name = "Bobby Tables", PetId = 123, Strength = 50, UserId = id };
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]Pet value)
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
