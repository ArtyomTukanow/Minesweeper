using Newtonsoft.Json;
using UserData.Rating;

namespace UserData
{
    public class UserData
    {
        [JsonProperty("sett")]
        public UserSettings Settings { get; private set; } = new UserSettings();
        
        [JsonProperty("rating")]
        public UserRating Rating { get; private set; } = new UserRating();
        
        [JsonProperty("lvl")]
        public UserLevels Levels { get; private set; } = new UserLevels();
    }
}