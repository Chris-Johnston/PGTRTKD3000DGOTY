using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PetGame.Core;
using PetGame.Models;

namespace PetGame
{
    [Route("api/[controller]")]
    public class LeaderboardController : Controller
    {
        private readonly SqlManager sqlManager;

        [HttpGet]
        public IActionResult Get()
        {
            //create list to hold the races
            //List<Race> TopTenRaces = new List<Race>();
            List<string> TopTenRaces = new List<string>();

            //Query db for races
            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();

                cmd.CommandText = @"SELECT TOP (10) Pet.[Name] AS 'Pet Name', Race.Score, [User].Username AS 'Owner Name' FROM Pet, Race, [User] WHERE Race.PetId = Pet.PetId AND [User].UserId = Pet.UserId ORDER BY Score DESC;";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            TopTenRaces.Add(reader.GetString(i));
                        }
                        
                    }
                }
            }
            //return races
            return Json(TopTenRaces);
        }
    }
}
