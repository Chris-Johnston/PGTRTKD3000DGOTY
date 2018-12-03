using Microsoft.AspNetCore.Mvc;
using PetGame.Core;
using PetGame.Models;

namespace PetGame
{
    [Route("api/[controller]")]
    public class LeaderboardController : Controller
    {
        private readonly SqlManager sqlManager;
        private readonly LeaderboardService LeaderboardService;

        public LeaderboardController(SqlManager sqlManager)
        {
            this.sqlManager = sqlManager;
            LeaderboardService = new LeaderboardService(sqlManager);
        }

        /// <summary>
        /// Returns a List of LeaderboardEntry as JSON.
        /// Can be offset and the number of results can be changed
        /// using a raw JSON request body
        /// </summary>
        /// <param name="Request"> 
        /// JSON request body containing offset and desired number of results
        /// Defaults to 0 offset and 10 results if no request body is present
        /// </param>
        /// <returns>
        /// Each LeaderboardEntry contains: Score, Timestamp, PetName, PetId
        /// OwnerName, OwnerId
        /// </returns>
        [HttpGet]
        public IActionResult Get([FromBody] LeaderboardRequestModel Request)
        {
            //if no request body is specified, it will be null
            //if null, creates a new request with default values
            if (Request == null)
            {
                Request = new LeaderboardRequestModel() { Offset = 0, NumRequests = 10 };
            }

            //if the offset is >= 0 and the request number is > 0, the request is valid
            if (Request.Offset >= 0 && Request.NumRequests > 0)
            {
                //get the list of entried by calling the function in LeaderboardService
                var Entries = LeaderboardService.GetLeaderboardEntries(Request.Offset, Request.NumRequests);

                //if the result is null, the target(s) were not found (404)
                if (Entries == null)
                {
                    return NotFound();
                }
                //otherwise return the result as JSON (200)
                else
                {
                    return Json(Entries);
                }
            }
            //if the request is invalid, return a 400
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets a single LeaderboardEntry by RaceId. No request body
        /// </summary>
        /// <param name="id">
        /// Specified in the url when calling the API
        /// i.e. /api/leaderboard/[RaceIdHere]
        /// </param>
        /// <returns>
        /// Each LeaderboardEntry contains: Score, Timestamp, PetName, PetId
        /// OwnerName, OwnerId
        /// </returns>
        [HttpGet("{id}")]
        public IActionResult Get(ulong id)
        {
            //call the Service function to get the entry
            var Entry = LeaderboardService.GetLeaderboardEntryByRaceId(id);

            //if the entry is not null, return it as Json, otherwise, return 404
            if (Entry == null)
            {
                return NotFound();
            }
            else
            {
                return Json(Entry);
            }
        }
    }
}
