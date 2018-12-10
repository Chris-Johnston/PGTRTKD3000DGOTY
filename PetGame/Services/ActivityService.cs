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
                after = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));

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
                    cmd.Parameters.AddWithValue("@Type", (char)type.Value);
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
                            // timestamp is in UTC
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
                Timestamp = DateTime.UtcNow,
                Type = type
            });
        }

        /// <summary>
        ///     Updates a pet as a result of a given activity
        /// </summary>
        /// <param name="petid"></param>
        /// <param name="petService"></param>
        /// <returns></returns>
        public void UpdatePetFromActivity(ActivityType activity, ulong petid, PetService petService)
        {
            // ensure we are only working with activitytype that we care about
            if (!(activity == ActivityType.Training || activity == ActivityType.Race || activity == ActivityType.RaceHighScore))
                return;

            var a = new List<Activity>(GetActivities(petid, 1, DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5)), activity));
            // check to see if this happened in the last 5 minutes for safety
            // other than when we just inserted it
            if (a.Count > 1)
                return;

            // hasn't happened in the last 5 minutes or however long, so we can update the pet
            // get the pet
            var pet = petService.GetPetById(petid);
            if (pet == null)
                return;

            switch (activity)
            {
                case ActivityType.Training:
                    pet.Endurance = Constrain(pet.Endurance + TrainingEnduranceBoost);
                    pet.Strength = Constrain(pet.Strength + TrainingStrengthBoost);
                    break;
                case ActivityType.Race:
                    pet.Endurance = Constrain(pet.Endurance + RaceEnduranceBoost);
                    pet.Strength = Constrain(pet.Strength + RaceStrengthBoost);
                    break;
                case ActivityType.RaceHighScore:
                    pet.Endurance = Constrain(pet.Endurance + (RaceEnduranceBoost * RaceMultiplier));
                    pet.Strength = Constrain(pet.Strength + (RaceStrengthBoost * RaceMultiplier));
                    break;
                default: return;
            }

            // if one of those three happened, update the pet
            petService.UpdatePet(petid, pet);
        }

        // boost to these attributes for Training
        public const int TrainingEnduranceBoost = 3;
        public const int TrainingStrengthBoost = 3;
        // for a race
        public const int RaceEnduranceBoost = 7;
        public const int RaceStrengthBoost = 7;
        // high score multiplier, if you get a high score, boosts this by a bit
        public const int RaceMultiplier = 2;

        private int Constrain(int value, int min = 0, int max = 100)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
