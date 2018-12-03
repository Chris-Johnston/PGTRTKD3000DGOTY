namespace PetGame.Models
{
    /// <summary>
    /// Represents the request body for the Leaderboard API
    /// </summary>
    public class LeaderboardRequestModel
    {
        public int Offset { get; set; }
        public int NumRequests { get; set; }
    }
}
