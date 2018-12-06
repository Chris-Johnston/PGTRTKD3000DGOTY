using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame.Models
{
    /// <summary>
    ///     Represents each race's outcome.
    /// </summary>
    [JsonObject]
    public class Race
    {
        /// <summary>
        ///     Gets or sets the Race's unique identifier.
        /// </summary>
        [JsonProperty]
        public ulong RaceId { get; set; }

        /// <summary>
        ///     Gets or sets the score.
        /// </summary>
        [JsonProperty]
        public int Score { get; set; }

        /// <summary>
        ///     Gets or sets the time when the score was set.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        /// <summary>
        ///     Gets or sets the Pet Id that placed this score.
        /// </summary>
        [JsonProperty]
        public ulong PetId { get; set; }
    }
}
