using Newtonsoft.Json;

namespace UserData.Rating
{
    public class UserRatingPlace
    {
        [JsonProperty("place")]
        public int Place { get; private set; }
        
        [JsonProperty("date")]
        public long Date { get; private set; }
        
        [JsonProperty("time")]
        public int Time { get; private set; }

        public void CorrectPlace(int newPlace) => Place = newPlace;

        public UserRatingPlace()
        {
        }
        
        public UserRatingPlace(long date, int time)
        {
            Date = date;
            Time = time;
        }
    }
}