using System;
using System.Collections.Generic;
using Controller.Map;
using Core;
using Newtonsoft.Json;
using UnityEngine;

namespace UserData
{
    public class UserSettings
    {
        public UserSettings()
        {
        }

        [JsonProperty("sett")]
        private Dictionary<string, string> settingsKeys = new Dictionary<string, string>();

        public void Set(string key, int val) => val.ToString();
        public void Set(string key, string val)
        {
            if(settingsKeys.ContainsKey(key) && settingsKeys[key].Equals(val))
                return;
            
            settingsKeys[key] = val;
            Game.User.Save();
        }
        
        public int GetInt(string key) => int.TryParse(GetString(key), out var t) ? t : default;
        public string GetString(string key)
        {
            if (!settingsKeys.ContainsKey(key))
                return default;
            return settingsKeys[key];
        }
        
        [JsonIgnore]
        private bool? rateUs;
        [JsonIgnore]
        public bool RateUs
        {
            get => rateUs ??= PlayerPrefs.GetInt("rate_us", 0) == 1;
            set
            {
                if (rateUs == value)
                    return;
                
                rateUs = value;
                PlayerPrefs.SetInt("rate_us", value ? 1 : 0);
            }
        }
        
        [JsonIgnore]
        private bool? vip;
        [JsonIgnore]
        public bool Vip
        {
            get => vip ??= PlayerPrefs.GetInt("vip", 0) == 1;
            set
            {
                if (vip == value)
                    return;
                
                vip = value;
                PlayerPrefs.SetInt("vip", value ? 1 : 0);
            }
        }
        
        [JsonIgnore]
        private int? level;
        [JsonIgnore]
        public int Level
        {
            get => level ??= PlayerPrefs.GetInt("game_level", 1);
            set
            {
                if (level == value)
                    return;
                
                level = value;
                PlayerPrefs.SetInt("game_level", value);
            }
        }
        
        [JsonIgnore]
        private float? zoom;
        [JsonIgnore]
        public float Zoom
        {
            get => zoom ??= PlayerPrefs.GetFloat("game_zoom", 4f);
            set
            {
                if (zoom == value)
                    return;
                
                zoom = value;
                PlayerPrefs.SetFloat("game_zoom", value);
            }
        }
        
        [JsonIgnore]
        private bool? blackPalette;
        [JsonIgnore]
        public bool BlackPalette
        {
            get => blackPalette ??= PlayerPrefs.GetInt("palette", 0) == 1;
            set
            {
                if (blackPalette == value)
                    return;
                
                blackPalette = value;
                PlayerPrefs.SetInt("palette", value ? 1 : 0);
                MapController.Instance?.UserMap.ForceUpdateState();
            }
        }

        public event Action OnLocalesChanged;

        [JsonIgnore]
        private SystemLanguage? language;
        [JsonIgnore]
        public SystemLanguage Language
        {
            get => language ??= Application.systemLanguage;
            set
            {
                if (language != value)
                {
                    language = value;
                    
                    OnLocalesChanged?.Invoke();
                    Game.User.Save();
                }
            }
        }
    }
}