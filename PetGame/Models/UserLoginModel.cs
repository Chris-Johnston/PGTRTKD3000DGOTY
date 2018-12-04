using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame.Models
{
    [JsonObject]
    public class UserLoginModel
    {
        [JsonProperty]
        public string username { get; set; }
        [JsonProperty]
        public string password { get; set; }
    }
}
