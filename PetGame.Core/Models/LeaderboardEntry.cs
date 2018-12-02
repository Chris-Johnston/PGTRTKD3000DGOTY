using System;
using System.Collections.Generic;
using System.Text;

namespace PetGame.Models
{
    public class LeaderboardEntry
    {
        public string PetName { get; set; }
        public int Score { get; set; }
        public string OwnerName { get; set; }
    }
}
