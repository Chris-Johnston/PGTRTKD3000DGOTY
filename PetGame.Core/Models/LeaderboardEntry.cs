using System;
using System.Collections.Generic;
using System.Text;

namespace PetGame.Models
{
    public class LeaderboardEntry
    {
        public string PetName { get; set; }
        public int score { get; set; }
        public string OwnerName { get; set; }

        public LeaderboardEntry(string nPetName, int nScore, string nOwnerName)
        {
            PetName = nPetName;
            score = nScore;
            OwnerName = nOwnerName;
        }
    }
}
