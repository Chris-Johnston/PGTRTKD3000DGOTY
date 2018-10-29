using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame.Models
{
    /// <summary>
    ///     Represents a Pet.
    /// </summary>
    public class Pet
    {
        /// <summary>
        ///     Gets or sets the unique Id of this pet.
        /// </summary>
        public ulong PetId { get; set; }

        //TODO: Make a regex rule for pet names and apply it to the setter of the Name property

        /// <summary>
        ///     Gets or sets the name of this pet.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the birthday (and time) of when this pet was created.
        /// </summary>
        public DateTime Birthday { get; set; }

        const int MaxStrength = 100;
        const int MinStrength = 0;

        /// <summary>
        ///     Gets or sets the strength of this pet, as a value between 0 and 100.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if the supplied value is out of the bounds of valid Strength values.
        /// </exception>
        public int Strength
        {
            get => _strength;
            set
            {
                // set the value while checking bounds
                if (value >= MinStrength && value <= MaxStrength)
                    _strength = value;
                else
                    throw new ArgumentOutOfRangeException(nameof(value), $"Strength must be in the bounds [{MinStrength}-{MaxStrength}].");
            }
        }
        
        // backing field for the Strength property
        private int _strength = MinStrength;

        const int MaxEndurance = 100;
        const int MinEndurance = 0;

        public int Enduracnce
        {
            get => _endurance;
            set
            {
                if (value >= MinEndurance && value <= MaxEndurance)
                    _endurance = value;
                else
                    throw new ArgumentOutOfRangeException(nameof(value), $"Endurance must be in the bounds [{MinEndurance}-{MaxEndurance}]");
            }
        }
        private int _endurance;

        /// <summary>
        ///     Gets or sets if the pet is dead or not.
        /// </summary>
        public bool IsDead { get; set; } = false;

        /// <summary>
        ///     Gets or sets the User Id of this Pet's owner.
        /// </summary>
        public ulong UserId { get; set; }
    }
}
