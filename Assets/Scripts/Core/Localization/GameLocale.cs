using Newtonsoft.Json;
using UnityEngine;

namespace Core.Localization
{
    public struct GameLocale
    {
        [JsonProperty("key")]
        public string Key { get; private set; }
        
        [JsonProperty("eng")]
        public string English { get; private set; }
        
        [JsonProperty("rus")]
        public string Russian { get; private set; }
        
        // [JsonProperty("deu")]
        // public string Germany { get; private set; }
        //
        // [JsonProperty("ita")]
        // public string Italian { get; private set; }
        //
        // [JsonProperty("jpn")]
        // public string Japanese { get; private set; }
        //
        // [JsonProperty("kor")]
        // public string Korean { get; private set; }
        //
        // [JsonProperty("zho")]
        // public string Chinese { get; private set; }
        //
        // [JsonProperty("spa")]
        // public string Spanish { get; private set; }
        //
        // [JsonProperty("fra")]
        // public string French { get; private set; }

        public string GetLocale() => GetLocale(Game.User.Settings.Language);
        public string GetLocale(SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.Russian:     return Russian;
                // case SystemLanguage.German:      return Germany;
                // case SystemLanguage.Italian:     return Italian;
                // case SystemLanguage.Japanese:    return Japanese;
                // case SystemLanguage.Korean:      return Korean;
                // case SystemLanguage.Chinese:     return Chinese;
                // case SystemLanguage.Spanish:     return Spanish;
                default:                         return English;
            }
        }
    }
}