using Controller;
using Core;
using UnityEngine;

namespace Mobile
{
    public class MobilePlatform
    {
        public RateUsController RateUsController { get; }

        public MobilePlatform()
        {
            RateUsController = Game.Instance.gameObject.AddComponent<RateUsController>();
        }
        
        public string GetPackageName()
        {
#if UNITY_IOS
			return Game.Consts.AppId;
#elif UNITY_WSA
			return GameConsts.WINDOWS_STORE_PRODUCT_ID;
#endif
            return Application.identifier;
        }

        public void GoToStore(string packageName = null)
        {
            string shopUrl = "market://details?id=";
	        
            packageName ??= GetPackageName();

#if UNITY_IOS
			shopUrl = "itms-apps://itunes.apple.com/app/id";
#elif UNITY_ANDROID
            shopUrl = "market://details?id=";
#elif UNITY_WSA
			shopUrl = "ms-windows-store://pdp/?productid=";
#endif
            Application.OpenURL(shopUrl + packageName);
        }
    }
}