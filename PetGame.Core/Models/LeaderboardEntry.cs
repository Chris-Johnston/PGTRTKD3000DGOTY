using System;
using System.Collections.Generic;
using System.Text;

namespace PetGame.Models
{
    public class LeaderboardEntry
    {
        public int Score { get; set; }
        public DateTime Timestamp { get; set;}
        public string PetName { get; set; }
        public ulong PetId { get; set; }
        public string OwnerName { get; set; }
        public ulong OwnerId { get; set; }
    }
}
