using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame.Models
{
    /// <summary>
    ///     Represents a Pet's activity.
    /// </summary>
    [JsonObject]
    public class Activity
    {
        /// <summary>
        ///     Gets or sets the unique identifier for this specific action.
        /// </summary>
        [JsonProperty]
        public ulong ActivityId { get; set; }

        /// <summary>
        ///     Gets or sets the Pet Id that this action is for.
        /// </summary>
        [JsonProperty]
        public ulong PetId { get; set; }

        /// <summary>
        ///     Gets or sets when this activity occured.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        ///     Gets or sets the type of activity performed.
        /// </summary>
        [JsonProperty]
        public ActivityType Type { get; set; } = ActivityType.Default;
    }
}
