using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame.Models
{
    /// <summary>
    ///     Represents a Pet's activity.
    /// </summary>
    public class PetActivity
    {
        /// <summary>
        ///     Gets or sets the unique identifier for this specific action.
        /// </summary>
        public ulong ActivityId { get; set; }

        /// <summary>
        ///     Gets or sets the Pet Id that this action is for.
        /// </summary>
        public ulong PetId { get; set; }

        /// <summary>
        ///     Gets or sets when this activity occured.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        ///     Gets or sets the type of activity performed.
        /// </summary>
        public ActivityType Type { get; set; } = ActivityType.Default;
    }
}
