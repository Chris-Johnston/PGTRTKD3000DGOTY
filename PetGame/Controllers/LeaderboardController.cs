using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PetGame.Core;
using PetGame.Models;

namespace PetGame
{
    [Route("api/[controller]")]
    public class LeaderboardController : Controller
    {
        //initialize connection to SQL Server
        private readonly SqlManager sqlManager;

        public LeaderboardController(SqlManager sqlManager)
        {
            this.sqlManager = sqlManager;
        }

        [HttpGet]
        public IActionResult Get(int NumResults = 10)
        {
            //create list to hold the information
            //names of pets on leaderboard
            List<LeaderboardEntry> ScoreList = new List<LeaderboardEntry>();
            //Query db for races
            using (var conn = sqlManager.EstablishDataConnection)
            {
                //create and set the SQL query
                var cmd = conn.CreateCommand();

                //to be optimized later, if time allows
                cmd.CommandText = @"SELECT TOP (" + NumResults + ") Pet.[Name] AS 'PetName', Race.Score, [User].Username AS 'OwnerName' " +
                    "FROM Pet, Race, [User] WHERE Race.PetId = Pet.PetId AND [User].UserId = Pet.UserId ORDER BY Score DESC;";

                //store the pet names, scores, and owner names in the lists
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string PetName = reader.GetString(0);
                        int score = reader.GetInt32(1);
                        string OwnerName = reader.GetString(2);

                        //add each pet name/score/owner name to a new index in the list
                        ScoreList.Add(new LeaderboardEntry(PetName, score, OwnerName));
                    }
                }
            }
            //return races
            return Json(ScoreList);
        }
    }
}
