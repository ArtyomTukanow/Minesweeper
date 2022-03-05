using System.Collections.Generic;
using Core.AssetsManager;
using Core.Localization;
using ModelData.IAP;
using Newtonsoft.Json;
using UnityEngine;

namespace ModelData
{
    public class StaticData
    {
        public const string PRODUCT_DATA_NAME = "products";
        public const string LOCALES_NAME = "langs";
        
        public List<IAPProductData> Products { get; private set; }
        public GameLocalization Localization { get; private set; }

        public void LoadAndParse(string name) => Parse(name, AssetsLoader.LoadSync<TextAsset>("Model/" + name).text);
        
        public void Parse(string name, string jsonString)
        {
            switch (name)
            {
                case PRODUCT_DATA_NAME:
                    Products = JsonConvert.DeserializeObject<List<IAPProductData>>(jsonString);
                    break;
                case LOCALES_NAME:
                    Localization = new GameLocalization(jsonString);
                    break;
            }
        }
    }
}