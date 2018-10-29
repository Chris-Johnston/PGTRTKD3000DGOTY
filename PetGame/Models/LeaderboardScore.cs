using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame.Models
{
    /// <summary>
    ///     Represents a score on the leaderboard.
    /// </summary>
    public class LeaderboardScore
    {
        /// <summary>
        ///     Gets or sets the score's unique identifier.
        /// </summary>
        public ulong ScoreId { get; set; }

        /// <summary>
        ///     Gets or sets the score.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        ///     Gets or sets the time when the score was set.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        ///     Gets or sets the Pet Id that placed this score.
        /// </summary>
        public ulong PetId { get; set; }
    }
}
