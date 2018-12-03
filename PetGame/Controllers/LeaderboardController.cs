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
        /// 
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get([FromBody] LeaderboardRequestModel Request)
        {
            if (Request == null)
            {
                Request = new LeaderboardRequestModel() { NumRequests = 10, Offset = 3 };
            }

            if (Request.NumRequests > 0)
            {
                var Entries = LeaderboardService.GetLeaderboardEntries(Request.NumRequests);

                if (Entries == null)
                {
                    return NotFound();
                }
                else
                {
                    return Json(Entries);
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(ulong id)
        {
            if (!(id < 0))
            {
                var Entry = LeaderboardService.GetLeaderboardEntryByRaceId(id);

                if (Entry == null)
                {
                    return NotFound();
                }
                else
                {
                    return Json(Entry);
                }
            }
            else
            {
                return BadRequest();
            }     
        }
    }
}
