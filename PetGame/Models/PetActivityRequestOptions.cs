using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetGame.Models
{
    [JsonObject]
    public class PetActivityRequestOptions
    {
        [JsonProperty]
        public int Limit { get; set; }
        [JsonProperty]
        public DateTime After { get; set; }
        [JsonProperty]
        public char Type { get; set; }

        public ActivityType FixedType => (ActivityType)Type;
        public PetActivityRequestOptions()
        {
            Limit = 10;
            After = DateTime.Now.Subtract(TimeSpan.FromDays(1));
            Type = (char) ActivityType.Default;
        }
    }
}
