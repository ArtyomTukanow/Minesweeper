using AllUtils;
using ModelData.Advert;
using Newtonsoft.Json;
using UserData.IAP;
using UserData.Rating;

namespace UserData
{
    public class User
    {
        private const string USER_DATA_FILE = "user.data";

        public int Level
        {
            get => Settings.Level;
            set => Settings.Level = value;
        }
        
        public bool IsEng => false;//todo
        
        public UserData Data { get; private set; }

        public UserSettings Settings => Data.Settings;
        public UserRating Rating => Data.Rating;
        public UserLevels Levels => Data.Levels;
        
        public UserAdvert Advert { get; private set; }
        public UserProducts Products { get; private set; }
        public UserPurchaseHandler PurchaseHandler { get; private set; }

        public User()
        {
        }

        public void Init()
        {
            Load();
            Advert = new UserAdvert();
            Products = new UserProducts();
            PurchaseHandler = new UserPurchaseHandler();
        }

        public void Save()
        {
            AllUtils.Utils.SaveFile(USER_DATA_FILE, JsonConvert.SerializeObject(Data));
        }

        private void Load()
        {
            var userString = AllUtils.Utils.ReadFile<string>(USER_DATA_FILE);
            
            if (userString.IsNullOrEmpty())
                Data = new UserData();
            else
                Data = JsonConvert.DeserializeObject<UserData>(userString);
        }
    }
}