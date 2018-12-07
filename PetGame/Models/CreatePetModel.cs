using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame.Models
{
    [JsonObject]
    public class CreatePetModel
    {
        public CreatePetModel() { }
        [JsonProperty]
        public string PetName { get; set; } = null;
        [JsonProperty]
        public int PetImageId { get; set; } = 0;
    }
}
