using PetGame.Core;
using PetGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame.Services
{
    /// <summary>
    ///     Util methods for managing Pets.
    /// </summary>
    public class PetService
    {
        //the amount to subtract from the hunger value every hour
        const double HungerDecreasePerHour = 0.05;
        //the amount to add to the hunger value on each feeding action
        const double FeedingIncrease = 0.10;
        //the amount to subtract from the hunger value on each training event
        const double TrainingDecrease = 0.15;
        //the amount to subtract from the hunger value on each race event
        const double RaceDecrease = 0.20;
        //the amount to subtract from the happiness value every hour
        const double HappinessDecreasePerHour = 0.07;
        //the number of activities to check
        const int NumActivities = 10;
        //the number of days prior to check
        const int NumDaysToCheck = 1;
        //the min percentage for hunger and happiness
        const double MinimumPercentage = 0.0;
        //the max percentage for hunger and happiness
        const double MaximumPercentage = 1.0;
        //the number of minutes between each action
        const int CooldownLength = 5;

        private readonly SqlManager sqlManager;
        private readonly ActivityService activityService;

        public PetService(SqlManager _sqlManager)
        {
            this.sqlManager = _sqlManager;
            activityService = new ActivityService(this.sqlManager);
        }
        /// <summary>
        ///     Gets an instance of a pet by id, if it exists.
        /// </summary>
        /// <param name="petid">
        ///     The pet's id.
        /// </param>
        /// <returns>
        ///     An instance of this pet, or null if not found.
        /// </returns>
        public Pet GetPetById(ulong petid)
        {
            Pet p = null;
            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"SELECT PetId, Name, Birthday, Strength, Endurance, IsDead, UserId, PetImageId FROM Pet WHERE PetId = @PetId;";
                cmd.Parameters.AddWithValue("@PetId", $"{petid}");

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p = new Pet()
                        {
                            PetId = petid,
                            Name = reader.GetString(1),
                            Birthday = reader.GetDateTime(2),
                            Strength = reader.GetInt32(3),
                            Endurance = reader.GetInt32(4),
                            IsDead = reader.GetBoolean(5),
                            UserId = (ulong)reader.GetInt64(6),
                            PetImageId = reader.GetInt32(7)
                        };
                    }
                }
            }
            return p;
        }

        /// <summary>
        ///     Inserts a new pet into the database.
        /// </summary>
        /// <remarks>
        ///     The ID property of the pet must be left unset, because
        ///     this is only determined after it is inserted into the 
        ///     database.
        ///     The returned entity will have this property set.
        /// </remarks>
        /// <param name="pet">
        ///     The pet to insert into the database.
        /// </param>
        /// <returns>
        ///     A copy of the pet inserted into the database, with the ID property set.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the pet parameter is null.
        /// </exception>
        public Pet InsertPet(Pet pet)
        {
            if (pet == null)
            {
                throw new ArgumentNullException(nameof(pet), "A null pet cannot be inserted.");
            }

            using (var conn = sqlManager.EstablishDataConnection)
            {
                // create the insert command
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"INSERT INTO Pet (Name, Birthday, Strength, Endurance, IsDead, UserId, PetImageId)
                      OUTPUT INSERTED.PetId
                      VALUES (@Name, @Birthday, @Strength, @Endurance, @IsDead, @UserId, @PetImageId);";
                cmd.Parameters.AddWithValue("@Name", pet.Name);
                cmd.Parameters.AddWithValue("@Birthday", pet.Birthday);
                cmd.Parameters.AddWithValue("@Strength", pet.Strength);
                cmd.Parameters.AddWithValue("@Endurance", pet.Endurance);
                cmd.Parameters.AddWithValue("@IsDead", pet.IsDead);
                cmd.Parameters.AddWithValue("@UserId", $"{pet.UserId}");
                cmd.Parameters.AddWithValue("@PetImageId", $"{pet.PetImageId}");

                // read the ID that was inserted
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pet.PetId = (ulong) reader.GetInt64(0);
                    }
                }
            }

            return pet;
        }

        /// <summary>
        ///     Updates a pet in the database.
        /// </summary>
        /// <param name="id">The ID of the pet to update.</param>
        /// <param name="pet">The pet data to update.</param>
        /// <returns>
        ///     A copy of the instance of Pet that was updated.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the supplied pet paramter is null.
        /// </exception>
        public Pet UpdatePet(ulong id, Pet pet)
        {
            if (pet == null)
                throw new ArgumentNullException(nameof(pet), "The value of Pet may not be null.");

            Pet ret = null;

            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"UPDATE Pet SET Name = @Name, Birthday = @Birthday, Strength = @Strength, Endurance = @Endurance, IsDead = @IsDead, UserId = @UserId, PetImageId = @PetImageId
                     WHERE PetId = @PetId;";
                cmd.Parameters.AddWithValue("@Name", pet.Name);
                cmd.Parameters.AddWithValue("@Birthday", pet.Birthday);
                cmd.Parameters.AddWithValue("@Strength", pet.Strength);
                cmd.Parameters.AddWithValue("@Endurance", pet.Endurance);
                cmd.Parameters.AddWithValue("@IsDead", pet.IsDead);
                cmd.Parameters.AddWithValue("@UserId", $"{pet.UserId}");
                // use the separate ID provided, not the id from Pet
                cmd.Parameters.AddWithValue("@PetId", $"{id}");
                cmd.Parameters.AddWithValue("@PetImageId", pet.PetImageId);

                var results = cmd.ExecuteNonQuery();

                if (results == 0)
                {
                    // no rows affected
                }
                else
                {
                    // at least 1 row affected (hopefully only 1)
                    ret = new Pet()
                    {
                        PetId = id,
                        Name = pet.Name,
                        Birthday = pet.Birthday,
                        Strength = pet.Strength,
                        Endurance = pet.Endurance,
                        IsDead = pet.IsDead,
                        UserId = pet.UserId,
                        PetImageId = pet.PetImageId
                    };
                }
            }
            return ret;
        }

        /// <summary>
        ///     Deletes a pet by it's Id based on it's owner.
        /// </summary>
        /// <param name="petId">
        ///     The Id of the pet.
        /// </param>
        /// <param name="ownerId">
        ///     The Id of the pet's owner.
        /// </param>
        /// <returns>
        ///     True if the pet was deleted, false if it wasn't found
        ///     or if the user was unauthorized.
        /// </returns>
        public bool DeletePet(ulong petId, ulong ownerId)
        {
            int results = -1;
            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"DELETE FROM Pet WHERE PetId = @PetId and UserId = @UserId;";
                cmd.Parameters.AddWithValue("@PetId", $"{petId}");
                cmd.Parameters.AddWithValue("@UserId", $"{ownerId}");

                results = cmd.ExecuteNonQuery();
            }
            // return true if exactly one pet was deleted
            return results == 1;
        }

        /// <summary>
        /// Returns a PetStatus that contains the Pet, its hunger percentage,
        /// its happiness percentage, and the DateTime of next interaction
        /// </summary>
        /// <param name="PetId"></param>
        /// <returns>
        /// PetStatus object
        /// happiness: 0= saddest 1 = happiest
        /// hunger: 0= very hungry 1 = not hungry
        /// </returns>
        public PetStatus GetPetStatusById(ulong PetId)
        {
            //call PetService function to get the Pet
            Pet toReturn = GetPetById(PetId);

            //if this pet doesn't exist, don't go any further. return null
            if (toReturn == null)
            {
                return null;
            }

            //hunger starts at 100%
            double hungerPercentage = MaximumPercentage;

            //happiness starts at 100%
            double happinessPercentage = MaximumPercentage;

            DateTime ToCheck = DateTime.Now.Subtract(TimeSpan.FromDays(NumDaysToCheck));
            int HoursToCheck = NumDaysToCheck * 24;

            IEnumerable<Activity> PastActivities = activityService.GetActivities(PetId, NumActivities, ToCheck, null);

            //calculate hunger
            //if there are no activities, decrease hunger by number of hours
            //and ensure that hunger cannot be <0 or >1
            hungerPercentage = CalculateHunger(PastActivities, hungerPercentage, HoursToCheck);

            //check the number of hours since the last activity
            int HoursSinceLastActivity = 0;
            HoursSinceLastActivity = CalculateHoursSinceLastActivity(PastActivities, HoursSinceLastActivity, HoursToCheck);

            int MinutesToNextAction = CooldownLength;
            MinutesToNextAction = CalculateMinutesToNextAction(PastActivities, MinutesToNextAction, HoursToCheck);

            //multiply HappinessDecrease by the number of hours to get the
            //happiness percentage and ensure that happiness cannot be <0 or >1
            happinessPercentage = ConstrainPercentage(HoursSinceLastActivity * (-1 * HappinessDecreasePerHour));

            //compile the data into a new PetStatus object
            //return the new object
            return new PetStatus() { Pet = toReturn, Hunger = hungerPercentage, Happiness = happinessPercentage,
                TimeOfNextAction = DateTime.Now.AddMinutes(MinutesToNextAction)};
        }//end of function

        private int CalculateHoursSinceLastActivity(IEnumerable<Activity> PastActivities, int HoursSinceLastActivity, int HoursToCheck)
        {
            List<Activity> Activities = (List<Activity>)PastActivities;
            //check to see if there were any activities in the specified interval
            if (Activities.Count == 0)
            {
                HoursSinceLastActivity = HoursToCheck;
            }
            else
            {
                //get the last activity
                Activity LastActivity = Activities[0];

                //check the number of hours since the last activity
                TimeSpan span = DateTime.Now.Subtract(LastActivity.Timestamp);
                HoursSinceLastActivity = (int) span.TotalHours;
            }
            return HoursSinceLastActivity;
        }

        private int CalculateMinutesToNextAction(IEnumerable<Activity> PastActivities, int MinutesToNextAction, int HoursToCheck)
        {
            List<Activity> Activities = (List<Activity>)PastActivities;

            if (Activities.Count == 0)
            {
                MinutesToNextAction = HoursToCheck * 60;
            }
            else
            {
                Activity LastActivity = Activities[0];

                //check the number of minute since the last activity
                int MinutesSinceLastActivity = 0;
                TimeSpan span = DateTime.Now.Subtract(LastActivity.Timestamp);
                MinutesSinceLastActivity = (int)span.TotalMinutes;

                //if it's been more than 5 minutes, the user can perform another action
                //otherwise, leave the default value in place
                if (MinutesSinceLastActivity > CooldownLength)
                {
                    MinutesToNextAction = 0;
                }
            }
            return MinutesToNextAction;
        }

        private double CalculateHunger(IEnumerable<Activity> PastActivities, double hungerPercentage, int HoursToCheck)
        {
            List<Activity> Activities = (List<Activity>)PastActivities;

            //there are no activities in the time span, so decrease for each hour
            if (Activities.Count == 0)
            {
                hungerPercentage = ConstrainPercentage(hungerPercentage - (HungerDecreasePerHour * HoursToCheck));
            }
            else
            {
                //check the type of activity and subtract percentage accordingly
                foreach (Activity Activity in PastActivities)
                {
                    //if a Feeding activity has occured, increase hunger
                    if (Activity.Type.Equals(ActivityType.Feeding))
                    {
                        hungerPercentage = ConstrainPercentage(hungerPercentage + FeedingIncrease);
                    }
                    //if a Training event has occured, subtract
                    else if (Activity.Type.Equals(ActivityType.Training))
                    {
                        hungerPercentage = ConstrainPercentage(hungerPercentage - TrainingDecrease);
                    }
                    //if a Race event has occurred
                    else if (Activity.Type.Equals(ActivityType.Race))
                    {
                        hungerPercentage = ConstrainPercentage(hungerPercentage - RaceDecrease);
                    }
                    //else, decrease hunger
                    else
                    {
                        hungerPercentage = ConstrainPercentage(hungerPercentage - HungerDecreasePerHour);
                    }
                }
            }
            return hungerPercentage;
        }

        /// <summary>
        /// Checks that the supplied percentage is not
        /// greater than max, or less than min
        /// </summary>
        /// <param name="percentage">The percentage value to check</param>
        /// <returns>The percentage as a double</returns>
        private double ConstrainPercentage(double percentage)
        {
            //if the percentage is less than min, return min
            if (percentage < MinimumPercentage)
            {
                return MinimumPercentage;
            }
            //if the percentage is less than max, return max
            else if (percentage > MaximumPercentage)
            {
                return MaximumPercentage;
            }
            //otherwise, the percentage is valid, so return
            //it with no modification
            else
            {
                return percentage;
            }
        }

        /// <summary>
        /// Gets a List of the statuses of all a user's pets
        /// </summary>
        /// <param name="UserId">User's Id</param>
        /// <returns>List of PetStatus, with one entry per pet</returns>
        public IEnumerable<PetStatus> GetPetStatusList(ulong UserId)
        {
            //list of PetId's to retrieve
            List<ulong> PetIdList = new List<ulong>();

            //List of Pet Status to return
            List<PetStatus> PetStatusList = new List<PetStatus>();

            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();

                //get all the PetId's for Pets that belong to the specified User
                cmd.CommandText = @"SELECT Pet.PetId FROM PET WHERE Pet.UserId = @UserId;";

                cmd.Parameters.AddWithValue("@UserId", $"{UserId}");

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //store the returned PetIds
                        PetIdList.Add((ulong) reader.GetInt64(0));
                    }
                }

                //if the list of Ids is empty, the user has no pets. return null
                if (PetIdList.Count == 0)
                {
                    return null;
                }

                //get thw Status of each Pet by it's Id and add it to the list
                foreach (ulong Id in PetIdList)
                {
                    PetStatusList.Add(GetPetStatusById(Id));
                }
            }
            //return the list
            return (PetStatusList);
        }
    }
}
