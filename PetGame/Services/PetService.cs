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
        const double HungerDecreasePerHour = 0.05;
        const double HungerIncrease = 0.10;
        const double FeedingIncrease = 0.10;
        const double TrainingDecrease = 0.15;
        const double RaceDecrease = 0.20;
        const double HappinessDecreasePerHour = 0.07;
        const int NumActivities = 10;
        const int NumDaysToCheck = 1;

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
            double hungerPercentage = 1.0;

            //happiness starts at 100%
            double happinessPercentage = 1.0;

            DateTime ToCheck = DateTime.Now.Subtract(TimeSpan.FromDays(NumDaysToCheck));
            int HoursToCheck = NumDaysToCheck * 24;

            List<Activity> PastActivities = (List<Activity>)activityService.GetActivities(PetId, NumActivities, ToCheck, null);

            //calculate hunger
            //if there are no activities, dcrease hunger by number of hours
            if (PastActivities.Count == 0)
            {
                hungerPercentage -= (HungerDecreasePerHour * HoursToCheck);
            }
            else
            {
                //check the type of activity and subtract percentage accordingly
                foreach (Activity Activity in PastActivities)
                {
                    //if a Feeding activity has occured, increase hunger
                    if (Activity.Type.Equals(ActivityType.Feeding))
                    {
                        if (hungerPercentage < 1.0 &&
                            ((hungerPercentage += HungerIncrease) <= 1.0))
                        {
                            hungerPercentage += HungerIncrease;
                        }
                        else
                        {
                            hungerPercentage = 1.0;
                        }
                    }
                    //if a Training event has occured, subtract
                    else if (Activity.Type.Equals(ActivityType.Training))
                    {
                        if (hungerPercentage > 0 &&
                            ((hungerPercentage -= TrainingDecrease) > 0))
                        {
                            hungerPercentage -= TrainingDecrease;
                        }
                        else
                        {
                            hungerPercentage = 0;
                        }
                    }
                    //if a Race event has occurred
                    else if (Activity.Type.Equals(ActivityType.Race))
                    {
                        if (hungerPercentage > 0 &&
                            ((hungerPercentage -= RaceDecrease) > 0))
                        {
                            hungerPercentage -= RaceDecrease;
                        }
                        else
                        {
                            hungerPercentage = 0;
                        }
                    }
                    //else, decrease hunger
                    else
                    {
                        if (hungerPercentage > 0 &&
                            ((hungerPercentage -= HungerDecreasePerHour) > 0))
                        {
                            hungerPercentage -= HungerDecreasePerHour;
                        }
                        else
                        {
                            hungerPercentage = 0;
                        }
                    }
                }
            }//end

            //check the number of hours since the last activity
            int HoursSinceLastActivity = 0;
            int MinutesTillNextAction = 5;

            //check to see if there were any activities in the specified interval
            if (PastActivities.Count == 0)
            {
                HoursSinceLastActivity = HoursToCheck;
            }
            else
            {
                //get the last activity
                Activity LastActivity = PastActivities[PastActivities.Count - 1];

                //check the number of hours since the last activity
                HoursSinceLastActivity = DateTime.Now.Hour - LastActivity.Timestamp.Hour;

                //check the number of minute since the last activity
                int MinutesSinceLastActivity = 0;
                MinutesSinceLastActivity = DateTime.Now.Minute - LastActivity.Timestamp.Minute;

                //if it's been more than 5 minutes, the user can perform another action
                //otherwise, leave the default 5 min value in place
                if (MinutesSinceLastActivity > 5)
                {
                    MinutesTillNextAction = 0;
                }
            }

            //multiply HappinessDecrease by the number of hours to get the
            //happiness percentage
            happinessPercentage = (HoursSinceLastActivity * (-1 * HappinessDecreasePerHour));

            //ensure that happiness cannot be <0 or >1
            if (happinessPercentage < 0.0)
            {
                happinessPercentage = 0.0;
            }
            else if (happinessPercentage > 1.0)
            {
                happinessPercentage = 1.0;
            }

            //ensure that hunger cannot be <0 or >1
            if (hungerPercentage < 0.0)
            {
                hungerPercentage = 0.0;
            }
            else if (hungerPercentage > 1.0)
            {
                hungerPercentage = 1.0;
            }

            //compile the data into a new PetStatus object
            //return the new object
            return new PetStatus() { Pet = toReturn, Hunger = hungerPercentage, Happiness = happinessPercentage,
                TimeOfNextAction = DateTime.Now.AddMinutes(MinutesTillNextAction)};
        }//end of function

        public IEnumerable<PetStatus> GetPetStatusList(ulong UserId)
        {
            List<ulong> PetIdList = new List<ulong>();
            List<PetStatus> PetStatusList = new List<PetStatus>();

            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();

                cmd.CommandText = @"SELECT Pet.PetId FROM PET WHERE Pet.UserId = @UserId;";

                cmd.Parameters.AddWithValue("@UserId", $"{UserId}");

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PetIdList.Add((ulong) reader.GetInt64(0));
                    }
                }

                if (PetIdList.Count == 0)
                {
                    return null;
                }

                foreach (ulong Id in PetIdList)
                {
                    PetStatusList.Add(GetPetStatusById(Id));
                }
            }
            return (PetStatusList);
        }
    }
}
