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
        ///     Deletes a race by Race ID.
        /// </summary>
        /// <param name="id">The ID of the race to delete.</param>
        [HttpDelete("{id}")]
        public IActionResult DeleteRace(ulong id)
        {
            var result = service.DeleteRace(id);
            if (result)
                return Ok();
            return NotFound();
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

            var result = service.InsertRace(value);
            return Ok(result);
        }

        /// <summary>
        ///     Updates a race in the database.
        /// </summary>
        /// <param name="id">
        ///     The Id of the race to update.
        /// </param>
        /// <param name="value">
        ///     The values of the Race to update.
        ///     Ignores the RaceId property of this object.
        /// </param>
        /// <returns>
        ///     The updated Race object.
        /// </returns>
        [HttpPut("{id}")]
        public IActionResult PutRace(ulong id, [FromBody] Race value)
        {
            if (value == null)
                return BadRequest("The Race cannot be null.");

            var race = service.UpdateRace(id, value);
            if (race == null)
                return NotFound();
            return Ok(race);
        }
    }
}