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
        const double DefaultDecrease = 0.07;
        const double TrainingDecrease = 0.10;
        const double RaceDecrease = 0.15;
        const double HappinessDecreasePerHour = 0.06;
        const int HoursToCheck = 12;

        private readonly SqlManager sqlManager;

        public PetService(SqlManager _sqlManager)
        {
            this.sqlManager = _sqlManager;
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
                    @"SELECT PetId, Name, Birthday, Strength, Endurance, IsDead, UserId FROM Pet WHERE PetId = @PetId;";
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
                            UserId = (ulong)reader.GetInt64(6)
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
                    @"INSERT INTO Pet (Name, Birthday, Strength, Endurance, IsDead, UserId)
                      OUTPUT INSERTED.PetId
                      VALUES (@Name, @Birthday, @Strength, @Endurance, @IsDead, @UserId);";
                cmd.Parameters.AddWithValue("@Name", pet.Name);
                cmd.Parameters.AddWithValue("@Birthday", pet.Birthday);
                cmd.Parameters.AddWithValue("@Strength", pet.Strength);
                cmd.Parameters.AddWithValue("@Endurance", pet.Endurance);
                cmd.Parameters.AddWithValue("@IsDead", pet.IsDead);
                cmd.Parameters.AddWithValue("@UserId", $"{pet.UserId}");

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
                    @"UPDATE Pet SET Name = @Name, Birthday = @Birthday, Strength = @Strength, Endurance = @Endurance, IsDead = @IsDead, UserId = @UserId
                     WHERE PetId = @PetId;";
                cmd.Parameters.AddWithValue("@Name", pet.Name);
                cmd.Parameters.AddWithValue("@Birthday", pet.Birthday);
                cmd.Parameters.AddWithValue("@Strength", pet.Strength);
                cmd.Parameters.AddWithValue("@Endurance", pet.Endurance);
                cmd.Parameters.AddWithValue("@IsDead", pet.IsDead);
                cmd.Parameters.AddWithValue("@UserId", $"{pet.UserId}");
                // use the separate ID provided, not the id from Pet
                cmd.Parameters.AddWithValue("@PetId", $"{id}");

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
                        UserId = pet.UserId
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
        /// </returns>
        public PetStatus GetPetStatus(ulong PetId)
        {
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

            //
            using (var conn = sqlManager.EstablishDataConnection)
            {
                List<Activity> LastIntervalActivities = new List<Activity>();
                var cmd = conn.CreateCommand();

                //gets all activities from the last 2 hours for specified pet
                cmd.CommandText = @"SELECT Activity.ActivityId, Activity.PetId, Activity.Timestamp, Activity.Type 
                                    FROM Activity, Pet 
                                    WHERE Activity.Timestamp > DATEADD(HOUR, @Hours, GETDATE()) AND Pet.PetId = @PetId;";

                //specify PetId
                cmd.Parameters.AddWithValue("@PetId", $"{PetId}");
                cmd.Parameters.AddWithValue("@Hours", (-1 * HoursToCheck));

                //create and fill a list of the last two hours of activities for this pet
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //convert string activity type to enum object
                        string Type = reader.GetString(3);
                        //assume defualt value is "Default"
                        ActivityType ActivityTypeInput = ActivityType.Default;

                        if (Type.Equals("Training"))
                        {
                            ActivityTypeInput = ActivityType.Training;
                        }

                        LastIntervalActivities.Add(new Activity() { ActivityId = (ulong) reader.GetInt64(0),
                            PetId = (ulong) reader.GetInt64(1), Timestamp = reader.GetDateTime(2),
                            Type = ActivityTypeInput });
                    }
                }//end

                //calculate hunger
                //if there are no activities in the last 2 hrs, subtract 10%
                //5% per hour
                if (LastIntervalActivities.Count == 0)
                {
                    hungerPercentage -= (HungerDecreasePerHour * 2);
                }
                else
                {
                    //check the type of activity and subtract percentage accordingly
                    foreach (Activity Activity in LastIntervalActivities)
                    { 
                        //if only one hour has passed, subtract 5%
                        if ((Activity.Timestamp.Hour + 1).Equals(DateTime.Now))
                        {
                            hungerPercentage -= HungerDecreasePerHour;
                        }
                        //if a traing activity has occured, subtract 7%
                        else if (Activity.Type.Equals(ActivityType.Default))
                        {
                            hungerPercentage -= DefaultDecrease;
                        }
                        //if a race has occured, subtract 15%
                        else if (Activity.Type.Equals(ActivityType.Training))
                        {
                            hungerPercentage -= TrainingDecrease;
                        }
                        //TODO: Add condition for Race. Blocked by update of enum
                    }
                }//end

                if (hungerPercentage < 0.0)
                {
                    hungerPercentage = 0.0;
                }

                //check the number of hours since the last activity
                int HoursSinceLastActivity = 0;

                if (LastIntervalActivities.Count == 0)
                {
                    HoursSinceLastActivity = HoursToCheck;
                }
                else
                {
                    //get the last activity
                    Activity LastActivity = LastIntervalActivities[LastIntervalActivities.Count - 1];

                    //check the number of hours since the last activity
                    HoursSinceLastActivity = DateTime.Now.Hour - LastActivity.Timestamp.Hour;
                }

                //multiply HappinessDecrease by the number of hours and the hunger percentage to get the
                //happiness percentage
                happinessPercentage = (HoursSinceLastActivity * HappinessDecreasePerHour);

                if (happinessPercentage < 0.0)
                {
                    happinessPercentage = 0.0;
                }

                //compile the data into a new PetStatus object
                //return the new object
                return new PetStatus() { Pet = toReturn, Hunger = hungerPercentage, Happiness = happinessPercentage };
            }
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
                    PetStatusList.Add(GetPetStatus(Id));
                }
            }
            return (PetStatusList);
        }
    }
}
