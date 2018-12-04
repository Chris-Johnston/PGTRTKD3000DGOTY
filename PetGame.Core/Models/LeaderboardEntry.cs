using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetGame.Models
{
    /// <summary>
    /// A type that represents one entry (row) in the leaderboard
    /// </summary>
    [JsonObject]
    public class LeaderboardEntry
    {
        /// <summary>
        /// The Pet's Score
        /// </summary>
        [JsonProperty]
        public int Score { get; set; }

        /// <summary>
        /// The Timestamp corresponding to the race on the leaderboard
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// The Pet's name
        /// </summary>
        [JsonProperty]
        public string PetName { get; set; }

        /// <summary>
        /// The Pet's Id
        /// </summary>
        [JsonProperty]
        public ulong PetId { get; set; }

        /// <summary>
        /// The Owner's Name
        /// </summary>
        [JsonProperty]
        public string OwnerName { get; set; }

        /// <summary>
        /// The Owner's Id
        /// </summary>
        [JsonProperty]
        public ulong OwnerId { get; set; }
    }
}
