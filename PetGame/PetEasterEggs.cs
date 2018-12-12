using PetGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PetGame
{
    /// <summary>
    ///     If you are looking for all the easter eggs, this is where you'll find them.
    /// </summary>
    public class PetEasterEggs
    {
        const string GoldOrang = "The Golden God";
        const string HackerRegex = @"(.)+script(.)+alert";
        const string Thinking = "think";

        /// <summary>
        ///     Checks if a pet has a special name, and makes modifications to it accordingly.
        /// </summary>
        /// <param name="pet"></param>
        public static void CheckIfSpecialName(Pet pet, string ownerName)
        {
            if (pet.Name == GoldOrang)
            {
                pet.PetImageId = PetImages.ImageGoldOrangId;
                pet.Strength = 50;
                pet.Endurance = 50;
            }
            else if (Regex.IsMatch(pet.Name, HackerRegex))
            {
                pet.Name = "Hackerman";
                pet.Strength = 13;
                pet.Endurance = 37;
                pet.PetImageId = PetImages.ImageHacker;
            }
            else if (pet.Name.ToLowerInvariant().Contains(Thinking))
            {
                pet.Strength = 20;
                pet.Endurance = 20;
                pet.PetImageId = PetImages.ImageThinking;
            }
            else if (pet.Name.ToLowerInvariant() == "legs")
            {
                pet.Strength = 99;
                pet.Strength = 99;
                pet.PetImageId = PetImages.ImageLegs;
            }
            else if (pet.Name.ToLowerInvariant() == "vim" || Regex.IsMatch(pet.Name, "^[Rr]+[eE]+") || Regex.IsMatch(ownerName, "^[Rr]+[eE]+"))
            {
                pet.Strength = 99;
                pet.Strength = 99;
                pet.PetImageId = PetImages.ImageVim;
            }
        }
    }
}
