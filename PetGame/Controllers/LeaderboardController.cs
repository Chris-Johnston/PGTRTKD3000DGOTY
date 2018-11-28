using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PetGame.Models;

namespace PetGame
{
    [Route("api/[controller]")]
    public class LeaderboardController : Controller
    {
        [HttpGet]
        public IEnumerable<Race> Get()
        {
            //create list to hold the races
            List<Race> TopTenRaces = new List<Race>();

            //Query db for races
            
            //sort by score in descending order
            TopTenRaces.Sort((x, y) => y.Score.CompareTo(x.Score));

            //return races
            return TopTenRaces;
        }
    }
}
