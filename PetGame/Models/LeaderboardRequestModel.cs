namespace PetGame.Models
{
    /// <summary>
    /// Represents the request body for the Leaderboard API
    /// </summary>
    public class LeaderboardRequestModel
    {
        public uint Offset { get; set; }
        public uint NumRequests { get; set; }
    }
}
