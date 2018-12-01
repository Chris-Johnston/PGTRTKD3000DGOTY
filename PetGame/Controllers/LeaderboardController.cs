using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PetGame.Core;

namespace PetGame
{
    [Route("api/[controller]")]
    public class LeaderboardController : Controller
    {
        //initialize connection to SQL Server
        private readonly SqlManager sqlManager;

        public LeaderboardController(SqlManager sqlManager) : base()
        {
            this.sqlManager = sqlManager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            //create lists to hold the information
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
                //create and set the SQL query
                var cmd = conn.CreateCommand();

                //to be optimized later, if time allows
                cmd.CommandText = @"SELECT TOP (10) Pet.[Name] AS 'Pet Name', Race.Score, [User].Username AS 'Owner Name' 
                                    FROM Pet, Race, [User] 
                                    WHERE Race.PetId = Pet.PetId AND [User].UserId = Pet.UserId ORDER BY Score DESC;";

                //store the pet names, scores, and owner names in the lists
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //add each pet name/score/owner name to a new index in the list
                        TopTenPetNames.Add(reader.GetString(0));
                        TopTenScores.Add(reader.GetInt32(1));
                        TopTenOwnerNames.Add(reader.GetString(2));
                    }
                }

                //if any of the lists are empty, there is no usable data
                //return an empty table
                if (TopTenScores.Count <= 0 || TopTenPetNames.Count <= 0 ||
                        TopTenOwnerNames.Count <= 0)
                {
                    //returns empty table full of nulls
                    return Json(table);
                }
                //if the lists do not have equal lengths, there is no usable data
                //return an empty table
                else if ((TopTenScores.Count != TopTenPetNames.Count) ||
                            (TopTenPetNames.Count != TopTenOwnerNames.Count))
                {
                    //returns empty table full of nulls
                    return Json(table);
                }
                //if there is data, add it to the table
                else
                {
                    //add the data from the lists to the mulidimensional array/table
                    for (int i = 0; i < TopTenPetNames.Count; i++)
                    {
                        //add the pet names to index 0
                        table[0, i] = TopTenPetNames[i];
                        //add the scores to index 1
                        table[1, i] = TopTenScores[i].ToString();
                        //add the owner names to index 2
                        table[2, i] = TopTenOwnerNames[i];
                    }
                }
            }
            //return races
            return Json(table);
        }
    }
}
