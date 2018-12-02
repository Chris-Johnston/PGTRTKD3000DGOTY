using PetGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame.Services
{
    // handles getting and inserting activities
    public class ActivityService
    {
        /// <summary>
        ///     Gets all of the activities (up to the limit specified)
        ///     that occurred after the given time (default is 1 day ago)
        ///     for the given Pet Id, in order of most recent to least recent.
        /// </summary>
        /// <param name="petId">The ID of the pet to get the activities for.</param>
        /// <param name="limit">The limit of activities to get.</param>
        /// <param name="after">The cut-off for how recent activities must be. Defaults to 1 day ago.</param>
        /// <returns>An IEnumerable of all activities for this pet.</returns>
        public IEnumerable<Activity> GetActivities(ulong petId, int limit = 10, DateTime? after = null)
        {
            if (after == null)
                // get the datetime a day ago
                after = DateTime.Now.Subtract(TimeSpan.FromDays(1));

            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets an activity by Id.
        /// </summary>
        /// <param name="activityId">the ActivityId to get</param>
        /// <returns>
        ///     The value of the Activity, or null if not found.
        /// </returns>
        public Activity GetActivityById(ulong activityId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Inserts a new Activity into the database.
        /// </summary>
        /// <param name="activity"> The activity to insert. </param>
        /// <returns>
        ///     The activity object supplied with the ActivityId property set.
        /// </returns>
        public Activity InsertActivity(Activity activity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Makes a new Activity for the given pet and type,
        ///     and inserts it into the database.
        /// </summary>
        /// <param name="pet"> The pet to make the activity for. </param>
        /// <param name="type"> The type of Activity to make. </param>
        /// <returns>A new Activity object. </returns>
        public Activity MakeActivityForPet(ulong PetId, ActivityType type)
        {
            //TODO: Parameter validation
            return InsertActivity(new Activity()
            {
                ActivityId = 0,
                PetId = PetId,
                Timestamp = DateTime.Now,
                Type = type
            });
        }
    }
}
