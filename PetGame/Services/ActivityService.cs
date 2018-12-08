using PetGame.Core;
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
        private readonly SqlManager sqlManager;

        public ActivityService(SqlManager sql)
        {
            sqlManager = sql;
        }

        /// <summary>
        ///     Gets all of the activities (up to the limit specified)
        ///     that occurred after the given time (default is 1 day ago)
        ///     for the given Pet Id, in order of most recent to least recent.
        /// </summary>
        /// <param name="petId">The ID of the pet to get the activities for.</param>
        /// <param name="limit">The limit of activities to get.</param>
        /// <param name="after">The cut-off for how recent activities must be. Defaults to 1 day ago.</param>
        /// <param name="type">The type of activity to get. Set this to null to disable filtering by type.</param>
        /// <returns>An IEnumerable of all activities for this pet.</returns>
        public IEnumerable<Activity> GetActivities(ulong petId, int limit = 10, DateTime? after = null, ActivityType? type = null)
        {
            if (after == null)
                // get the datetime a day ago
                after = DateTime.Now.Subtract(TimeSpan.FromDays(1));

            // set the default limit to 10 entries
            if (limit == null)
                limit = 10;

            // list of the results to return
            List<Activity> ret = new List<Activity>();
            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();
                // no need to use OFFSET FETCH here, since there won't be any use cases where a paginated
                // view is necessary
                cmd.CommandText =
                    @"SELECT TOP (@Limit) ActivityId, PetId, Timestamp, Type
                        FROM Activity
                        WHERE PetId = @PetId AND Timestamp > @After AND (@Type IS NULL OR Type = @Type)
                        ORDER BY Timestamp DESC;";
                cmd.Parameters.AddWithValue("@PetId", $"{petId}");
                cmd.Parameters.AddWithValue("@After", after);
                cmd.Parameters.AddWithValue("@Limit", limit);
                // if type is null, then insert a null. this will disable filtering by type
                if (type.HasValue)
                {
                    cmd.Parameters.AddWithValue("@Type", type.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Type", DBNull.Value);
                }
                
                // read the results
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var n = new Activity()
                        {
                            ActivityId = (ulong)reader.GetInt64(0),
                            PetId = (ulong)reader.GetInt64(1),
                            Timestamp = reader.GetDateTime(2),
                            // hack: get the first character from this col, read as a string
                            Type = (ActivityType)reader.GetString(3)[0]
                        };
                        ret.Add(n);
                    }
                }
                // return all of the results
                return ret;
            }
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
            Activity ret = null;
            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"SELECT ActivityId, PetId, Timestamp, Type FROM Activity
                        FROM Activity
                        WHERE ActivityId = @ActivityId;";
                cmd.Parameters.AddWithValue("@ActivityId", $"{activityId}");

                // read the results
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret = new Activity()
                        {
                            ActivityId = (ulong)reader.GetInt64(0),
                            PetId = (ulong)reader.GetInt64(1),
                            Timestamp = reader.GetDateTime(2),
                            Type = (ActivityType)reader.GetChar(3)
                        };
                    }
                }
                return ret;
            }
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
            if (activity == null)
                throw new ArgumentNullException(paramName: nameof(activity),
                    message: "The supplied Activity cannot be null.");

            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"INSERT INTO Activity (PetId, Timestamp, Type)
                      OUTPUT INSERTED.ActivityId 
                      VALUES (@PetId, @Timestamp, @Type);";
                cmd.Parameters.AddWithValue("@PetId", $"{activity.PetId}");
                cmd.Parameters.AddWithValue("@Timestamp", activity.Timestamp);
                cmd.Parameters.AddWithValue("@Type", (char)activity.Type);

                // read the results
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        activity.ActivityId = (ulong) reader.GetInt64(0);
                    }
                }
                return activity;
            }
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
