using PetGame.Core;
using PetGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame.Services
{
    // security consideration: none of these endpoints require validation
    // because I'm not checking that the current user owns the pet that
    // is associated with the race.
    // probably an important check to make.
    // that being said, there's also no verification of results anyways,
    // and I highly doubt that there will be bad actors in play, so it
    // probably doesn't matter.
    public class RaceService
    {
        private readonly SqlManager sqlManager;

        public RaceService(SqlManager sql)
        {
            sqlManager = sql;
        }

        /// <summary>
        ///     Gets a Race by it's ID.
        /// </summary>
        /// <param name="raceid"> The RaceID to query the db for. </param>
        /// <returns>
        ///     A new <see cref="Race"/>, or null if not found.
        /// </returns>
        public Race GetRaceById(ulong raceid)
        {
            Race ret = null;
            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"SELECT Score, Timestamp, PetId FROM Race WHERE RaceId = @RaceId;";
                cmd.Parameters.AddWithValue("@RaceId", $"{raceid}");

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret = new Race()
                        {
                            RaceId = raceid,
                            Score = reader.GetInt32(0),
                            Timestamp = reader.GetDateTime(1),
                            PetId = (ulong)reader.GetInt64(2)
                        };
                    }
                }
            }

            return ret;
        }

        /// <summary>
        ///     Gets the ranking of a given race.
        /// </summary>
        /// <param name="raceid"></param>
        /// <returns></returns>
        public long GetRaceRank(ulong raceid)
        {
            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"SELECT Ranking
                    FROM (SELECT RaceId, RANK() OVER (ORDER BY Score DESC) AS Ranking FROM Race) R
                      WHERE R.RaceId = @RaceId;";
                cmd.Parameters.AddWithValue("@RaceId", $"{raceid}");

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader.GetInt64(0);
                    }
                }
            }
            return -1;
        }

        /// <summary>
        ///     Inserts a race into the database.
        /// </summary>
        /// <param name="race"> The race to insert into the database. </param>
        /// <returns>
        ///     A Race object where the RaceId property is set to the newly
        ///     inserted item's ID in the database.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the supplied Race is null.
        /// </exception>
        public Race InsertRace(Race race)
        {
            if (race == null)
                throw new ArgumentNullException(nameof(race), "A null race cannot be inserted.");

            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"INSERT INTO Race (Score, Timestamp, PetId)
                        OUTPUT INSERTED.RaceId
                        VALUES (@Score, @Timestamp, @PetId);";
                cmd.Parameters.AddWithValue("@Score", race.Score);
                cmd.Parameters.AddWithValue("@Timestamp", race.Timestamp);
                cmd.Parameters.AddWithValue("@PetId", $"{race.PetId}");

                // read the inserted ID
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        race.RaceId = (ulong)reader.GetInt64(0);
                    }
                }
            }
            return race;
        }

        /// <summary>
        ///     Updates a race in the database.
        /// </summary>
        /// <param name="id">The Id of the race to update.</param>
        /// <param name="race">The values of the race to update. Ignores this item's RaceId.</param>
        /// <returns>The updated Race object, null if not found. </returns>
        public Race UpdateRace(ulong id, Race race)
        {
            if (race == null)
                throw new ArgumentNullException(paramName: nameof(race), message: "The supplied Race cannot be null.");

            // return a new value instead of the same instance
            Race ret = null;

            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"UPDATE Race SET Score = @Score, Timestamp = @Timestamp, PetId = @PetId
                        WHERE RaceId = @RaceId";
                cmd.Parameters.AddWithValue("@Score", race.Score);
                cmd.Parameters.AddWithValue("@Timestamp", race.Timestamp);
                cmd.Parameters.AddWithValue("@PetId", $"{race.PetId}");
                cmd.Parameters.AddWithValue("@RaceId", $"{id}");

                var results = cmd.ExecuteNonQuery();
                if (results == 0)
                {
                    // no rows affected, so don't set ret
                }
                else
                {
                    ret = new Race()
                    {
                        RaceId = id,
                        Score = race.Score,
                        Timestamp = race.Timestamp,
                        PetId = race.PetId
                    };
                }
            }
            return ret;
        }
        
        /// <summary>
        ///     Deletes a race by id.
        /// </summary>
        /// <param name="id">The id of the race to delete.</param>
        /// <returns>True if the race was deleted, false otherwise (if not found).</returns>
        public bool DeleteRace(ulong id)
        {
            int results = -1;
            using (var conn = sqlManager.EstablishDataConnection)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"DELETE FROM Race WHERE RaceId = @RaceId;";
                cmd.Parameters.AddWithValue("@RaceId", $"{id}");

                results = cmd.ExecuteNonQuery();
            }
            return results == 1;
        }
    }
}
