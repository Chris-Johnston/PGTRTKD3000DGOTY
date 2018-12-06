using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame
{
    public enum ActivityType
    {
        /// <summary>
        ///     Placeholder type that should have no associated meaning.
        /// </summary>
        Default = 'd',
        /// <summary>
        ///     Indicates when a Pet was trained. Affects hunger, happiness, and permanent abilities like strength and endurance.
        /// </summary>
        Training = 't',
        /// <summary>
        ///     Indicates when a Pet was fed. Affects hunger, happiness, and the ability to perform actions.
        /// </summary>
        Feeding = 'f',
        //TODO: Consider adding a limit on how frequently UserLogin events are inserted into the database, to prevent users from spamming the database each time they view the pet
        /// <summary>
        ///     Indicates when a Pet has been visited by their owner. Affects happiness, and the ability to perform actions.
        /// </summary>
        UserLogin = 'u',
        /// <summary>
        ///     Indicates when a Pet has taken part in a Race. Affects hunger, and permanent abilities like strength and endurance.
        /// </summary>
        Race = 'r',
        /// <summary>
        ///     Indicates when a Pet's score from a race placed in the top 3 results. Temporarily maxes out their happiness.
        /// </summary>
        RaceHighScore = 's',
        //TODO: Needs implementing.
        /// <summary>
        ///     Indicates when a Pet is first created, tempoarily maxes out their happiness.
        /// </summary>
        FirstCreated = 'c'
    }
}
