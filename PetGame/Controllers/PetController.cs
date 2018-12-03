using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        private readonly ActivityService activityService;

        public PetController(SqlManager sqlManager)
        {
            this.sqlManager = sqlManager;
            petService = new PetService(this.sqlManager);
            loginService = new LoginService(this.sqlManager);
            activityService = new ActivityService(this.sqlManager);
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

        /// /api/Pet/[PetId]/status
        /// <summary>
        /// Returns the status of a single pet by its ID
        /// </summary>
        /// <param name="Request">
        /// JSON request body containing the Pet's ID
        /// </param>
        /// <returns>
        /// JSON of PetStatus object
        /// </returns>
        [HttpGet("{id}/status"), AllowAnonymous]
        public IActionResult Get(StatusRequest Request)
        {
            //PetStatus object to be serialized and returned
            PetStatus ret = petService.GetPetStatus(Request.id);
            
            //if not null, the call to GetPetStatus was successful
            if (ret != null)
            {
                return Json(ret);
            }
            //else return 404
            else
            {
                return NotFound();
            }
        }


        // in postman test with
        // where UserId matches the currently signed-in user
        /** POST /api/Pet
         * {
          "PetId": 123,
          "Name": "So fluffy boi",
          "Birthday": "2012-04-23T18:25:43.511Z",
          "Strength": 5,
          "Endurance": 55,
          "IsDead": true,
          "UserId": 35
            }
            */

        /// <summary>
        ///     Inserts a new Pet into the database.
        ///     This action requires authentication.
        /// </summary>
        /// <param name="value"> The pet to add to the database. </param>
        // POST api/<controller>
        [HttpPost]
        public IActionResult Post([FromBody] Pet value)
        {
            if (value == null)
                return BadRequest("The supplied Pet cannot be null.");
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
            if (value == null)
                return BadRequest("The supplied Pet cannot be null.");
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

        // Activity
        // GET /api/Pet/petId/Activity
        // POST /api/Pet/petId/ActivityOptions
        // {
        //"Limit": 10,
        //"After": "2012-04-23T18:25:43Z",
        //"Type": 'd'
        //   }
        [HttpGet("{petId}/Activity")]
        [HttpPost("{petId}/ActivityOptions")] // this must not be under /Activity, because that doesn't follow rest convention for POST
        public IActionResult GetRecentActivity(ulong petId, [FromBody] PetActivityRequestOptions options)
        {
            // if GET, or options not specified
            if (options == null)
                options = new PetActivityRequestOptions();

            // get all of the activities using the request options
            var results = activityService.GetActivities(petId, options.Limit, options.After, options.FixedType);
            if (results == null)
                return BadRequest();
            return Ok(results);
        }
        
        // POST /api/Pet/{petid}/Activity
        // {
//    "activityId": 0,
//    "petId": 0,
//    "timestamp": "2012-04-23T18:25:43Z",
//    "type": 116
//}
    // creates a new activity
    [HttpPost("{petId}/Activity")]
        public IActionResult PostNewActivity(ulong petid, Activity activity)
        {
            // ensure validity of params
            if (activity == null)
                return BadRequest();

            // enforce the pet id
            activity.PetId = petid;
            var result = activityService.InsertActivity(activity);
            if (result == null)
                return BadRequest();
            return Ok(result);
        }
        // TODO, need to require authorization, check that users may only modify their own pets

        // POST /api/Pet/{petId}/Activity/{type}
        // no request body required
        // creates a new activity of the given type, using the current time
        [HttpPost("{petId}/Activity/{type}")]
        public IActionResult PostNewActivity(ulong petid, char type)
        {
            ActivityType t = ActivityType.Default;
            try
            {
                t = (ActivityType)type;
            }
            catch (Exception)
            {
                // invalid type
                return BadRequest();
            }
            var result = activityService.MakeActivityForPet(petid, t);
            if (result == null)
                return BadRequest();
            return Ok(result);
        }
    }
}
