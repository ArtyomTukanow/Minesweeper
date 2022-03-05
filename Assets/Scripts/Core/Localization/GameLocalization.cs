using System;
using System.Collections.Generic;
using AllUtils;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Localization
{
    public class GameLocalization
    {
        private Dictionary<string, GameLocale> locales = new Dictionary<string, GameLocale>();
        
        public GameLocalization(string jsonText)
        {
            List<GameLocale> loc = JsonConvert.DeserializeObject<List<GameLocale>>(jsonText);
            foreach (var locale in loc)
            {
                if(locale.Key.IsNullOrEmpty())
                    continue;
                locales[locale.Key.ToLower()] = locale;
            }
        }

        public string GetLocale(string key, params string[] parameters) => GetLocale(Game.User.Settings.Language, key, parameters);
        public string GetLocale(SystemLanguage lang, string key, params string[] parameters)
        {
            key = key.ToLower();

            if (!locales.ContainsKey(key))
                return "$" + key;
            
            var result = locales[key].GetLocale(lang);
            
            if(result == null)
                return "$" + key;

            if (parameters != null && parameters.Length > 0)
                for (int i = 0; i < parameters.Length; i++)
                    result = result.Replace("@" + (i + 1), parameters[i] ?? String.Empty);

            result = result.Replace("<br>", Environment.NewLine);
            result = result.Replace("\\n", Environment.NewLine);
            
            return result;
        }
    }
}