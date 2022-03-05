using System;
using System.Collections.Generic;
using Assets.Scripts.Platform.Mobile.Advertising;
using Core;
using Mobile.Advetrising;

namespace ModelData.Advert
{
    public class UserAdvert
    {
        private readonly Dictionary<string, AbstractMobileAdvertising> partners = new Dictionary<string, AbstractMobileAdvertising>();

        public UserAdvert()
        {
            CreatePartners();
        }

        public void CreatePartners()
        {
            var admobData = new Dictionary<string, string>();

            admobData[AdmobAdvertising.GA_RWRD] = "ca-app-pub-9774871642897534/3069767924";
            if (!Game.User.Settings.Vip)
                admobData[AdmobAdvertising.GA_INT] = "ca-app-pub-9774871642897534/8288650023";
            
            partners[AdmobAdvertising.NAME] = AddAdmob();
            partners[AdmobAdvertising.NAME].UpdateData(admobData);
        }

        public void ShowReward(Action onComplete, Action onClose)
        {
            partners[AdmobAdvertising.NAME].ShowRewardAd(onComplete, onClose);
        }

        public void ShowInterstitial(Action onShow, Action onFailed)
        {
            partners[AdmobAdvertising.NAME].ShowInterstitial()
                .Then(b =>
                {
                    if (b)
                        onShow?.Invoke();
                    else
                        onFailed?.Invoke();
                });
        }

        private AbstractMobileAdvertising AddAdmob()
        {
            if (Game.Instance.gameObject.GetComponent<AdmobAdvertising>() is {} admob)
                return admob;
            return Game.Instance.gameObject.AddComponent<AdmobAdvertising>();
        }

        public bool IsRewardAvailable => partners[AdmobAdvertising.NAME]?.IsRewardLoadedReactive.Value == true;
        public bool IsInterstitialAvailable => partners[AdmobAdvertising.NAME]?.IsInterstitialLoadedReactive.Value == true;
    }
}