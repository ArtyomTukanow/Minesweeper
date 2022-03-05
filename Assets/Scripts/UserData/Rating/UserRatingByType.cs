using System.Collections.Generic;
using Core;
using ModelData.TileMap;
using Newtonsoft.Json;

namespace UserData.Rating
{
    public class UserRatingByType
    {
        private const int MAX_PLACES_IN_RATING = 100;
        
        [JsonProperty("t")]
        private int type;

        [JsonIgnore]
        public MapDataType Type => (MapDataType) type;
        
        [JsonProperty("places")] 
        public List<UserRatingPlace> Places { get; private set; } = new List<UserRatingPlace>();

        public UserRatingByType()
        {
        }

        public UserRatingByType(MapDataType type)
        {
            this.type = (int)type;
        }

        public void AddPlace(long date, int time) => AddPlace(new UserRatingPlace(date, time));
        public void AddPlace(UserRatingPlace place)
        {
            Places.Add(place);
            Places.Sort((p1, p2) => p1.Time - p2.Time);

            for (var i = 0; i < Places.Count; i++)
                Places[i].CorrectPlace(i + 1);

            CutPlacesToMax();
            
            Game.User.Save();
        }

        private void CutPlacesToMax()
        {
            if(Places.Count > MAX_PLACES_IN_RATING)
                Places.RemoveRange(MAX_PLACES_IN_RATING - 1, Places.Count - MAX_PLACES_IN_RATING);
        }
    }
}