using Newtonsoft.Json;

namespace PetGame.Models
{
    /// <summary>
    /// Represents the request body for the Leaderboard API
    /// </summary>
    [JsonObject]
    public class LeaderboardRequestModel
    {
        /// <summary>
        /// The number used in the OFFSET in the SQL query
        /// i.e. how many entries to skip
        /// </summary>
        [JsonProperty]
        public int Offset { get; set; }

        /// <summary>
        /// The number used in the FETCH in the SQL query
        /// i.e. how many entries to retrieve
        /// </summary>
        [JsonProperty]
        public int NumItems { get; set; }
    }
}
