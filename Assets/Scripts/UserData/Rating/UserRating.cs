using System.Collections.Generic;
using AllUtils.MergeUtil;
using ModelData.TileMap;
using Newtonsoft.Json;

namespace UserData.Rating
{
    public class UserRating
    {
        [JsonProperty("types")]
        private Dictionary<int, UserRatingByType> ratings = new Dictionary<int, UserRatingByType>();

        public UserRatingByType GetRatingByType(MapDataType type)
        {
            var intType = (int) type;
            if(!ratings.ContainsKey(intType))
                ratings[intType] = new UserRatingByType(type);
            return ratings[intType];
        }
    }
}