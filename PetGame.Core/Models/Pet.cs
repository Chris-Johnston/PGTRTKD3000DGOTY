using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PetGame.Models
{
    /// <summary>
    ///     Represents a Pet.
    /// </summary>
    [JsonObject]
    [DebuggerDisplay("Pet {PetId} {Name}")]
    public class Pet
    {
        public Pet() { Name = "Unnamed Pet"; }

        public Pet(string name, ulong userid)
        {
            Name = name;
            UserId = userid;
            PetId = 0;
            Birthday = DateTime.UtcNow;
            IsDead = false;
            // Easy TODO: Randomize the Strength and Endurance properties of pet when created for AddPet
            Strength = 5;
            Endurance = 5;
        }

        /// <summary>
        ///     Regular expression for valid pet names. All pet names must pass this validation.
        ///     Allows for names up to 50 char long that can contain spaces,
        ///     but must not start or end with whitespace.
        /// </summary>
        public const string NameRegex = @"^(['$@._/-?!0-9a-zA-Z])(['$@._/-?!0-9a-zA-Z ]){0,48}['$@._/-?!0-9a-zA-Z]$";

        /// <summary>
        ///     Gets or sets the unique Id of this pet.
        /// </summary>
        [JsonProperty]
        public ulong PetId { get; set; }

        /// <summary>
        ///     Gets or sets the name of this pet.
        /// </summary>
        [JsonProperty]
        public string Name
        {
            get => _Name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value), "A pet name cannot be null or whitespace.");
                }
                if (!Regex.IsMatch(value, NameRegex))
                {
                    throw new ArgumentException("A pet name must meet the requirements.", nameof(value));
                }
                _Name = value;
            }
        }
        private string _Name;

        /// <summary>
        ///     Gets or sets the birthday (and time) of when this pet was created.
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Birthday { get; set; }

        const int MaxStrength = 100;
        const int MinStrength = 0;

        /// <summary>
        ///     Gets or sets the strength of this pet, as a value between 0 and 100.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if the supplied value is out of the bounds of valid Strength values.
        /// </exception>
        [JsonProperty]
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

        [JsonProperty]
        public int Endurance
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
        [JsonProperty]
        public bool IsDead { get; set; } = false;

        /// <summary>
        ///     Gets or sets the User Id of this Pet's owner.
        /// </summary>
        [JsonProperty]
        public ulong UserId { get; set; }

        /// <summary>
        ///     Gets or sets the Id associated with the Pet's picture.
        /// </summary>
        [JsonProperty]
        public int PetImageId { get; set; }
    }
}
