using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PetGame.Core;

namespace PetGame
{
    [Route("api/[controller]")]
    public class LeaderboardController : Controller
    {
        private readonly SqlManager sqlManager;

        public LeaderboardController(SqlManager sqlManager) : base()
        {
            this.sqlManager = sqlManager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            //create lists to hold the informations
            //names of pets on leaderboard
            List<string> TopTenPetNames = new List<string>();
            //scores of pets on leaderboard
            List<Int32> TopTenScores = new List<Int32>();
            //owner names of pets on leaderboard
            List<string> TopTenOwnerNames = new List<string>();
            //data structure to be returned as json
            string[,] table = new string[3,10];
            //Query db for races
            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();

                cmd.CommandText = @"SELECT TOP (10) Pet.[Name] AS 'Pet Name', Race.Score, [User].Username AS 'Owner Name' FROM Pet, Race, [User] WHERE Race.PetId = Pet.PetId AND [User].UserId = Pet.UserId ORDER BY Score DESC;";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TopTenPetNames.Add(reader.GetString(0));
                        TopTenScores.Add(reader.GetInt32(1));
                        TopTenOwnerNames.Add(reader.GetString(2));
                    }
                }

                if (TopTenScores.Count <= 0 || TopTenPetNames.Count <= 0 ||
                    TopTenOwnerNames.Count <= 0)
                {
                    return Json("NoData");
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        table[0, i] = TopTenPetNames[i];
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        table[1, i] = TopTenScores[i].ToString();
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        table[2, i] = TopTenOwnerNames[i];
                    }
                }
            }
            //return races
            return Json(table);
        }
    }
}
