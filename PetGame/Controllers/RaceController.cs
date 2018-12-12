using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetGame.Core;
using PetGame.Models;
using PetGame.Services;

namespace PetGame
{
    [Route("api/[controller]")]
    [ApiController]
    public class RaceController : ControllerBase
    {
        private readonly SqlManager sqlManager;
        private readonly RaceService service;

        public RaceController(SqlManager sql)
        {
            sqlManager = sql;
            service = new RaceService(sqlManager);
        }
        
        /// <summary>
        ///     Gets a race by a Race ID.
        /// </summary>
        /// <param name="id"> The ID of the race to get.</param>
        /// <returns> The JSON represenation of a <see cref="Race"/>, if it exists. </returns>
        [HttpGet("{id}")]
        public IActionResult GetRace(ulong id)
        {
            var race = service.GetRaceById(id);
            if (race == null)
                return NotFound();
            return Ok(race);
        }
        
        /// <summary>
        ///     Inserts a new race into the database.
        /// </summary>
        /// <param name="value"> The Race to insert, from the request body. </param>
        /// <returns> The race that was inserted. </returns>
        [HttpPost]
        public IActionResult PostRace([FromBody] Race value)
        {
            if (value == null)
                return BadRequest();

            var loginService = new LoginService(this.sqlManager);
            var user = loginService.GetUserFromContext(HttpContext.User);
            if (user == null) return Unauthorized();

            var petService = new PetService(this.sqlManager);
            var pet = petService.GetPetById(value.PetId);
            if (pet == null) return NotFound();

            if (user.UserId != pet.UserId) return Unauthorized();

            var result = service.InsertRace(value);
            return Ok(result);
        }
    }
}