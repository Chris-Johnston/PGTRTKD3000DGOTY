using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame.Models
{
    /// <summary>
    ///     Represents each race's outcome.
    /// </summary>
    public class Race
    {
        /// <summary>
        ///     Gets or sets the Race's unique identifier.
        /// </summary>
        public ulong RaceId { get; set; }

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
