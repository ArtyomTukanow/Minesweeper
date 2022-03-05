using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using Mobile.Advetrising;
using UnityEngine;

namespace Assets.Scripts.Platform.Mobile.Advertising
{
    public class AdmobAdvertising : AbstractMobileAdvertising
    {
		public const string NAME = "admob";
        
        public const string GA_RWRD = "ga_rwrd";
        public const string GA_INT = "ga_int";
        
        protected override string Name => NAME;
        private AdRequest AdRequest => new AdRequest.Builder().Build();

		private string _adMobRewardId;
		private string _adMobInterstitialId;

		private RewardedAd _rewardAd;
		private InterstitialAd _interstitial;
        
        public override void UpdateData(Dictionary<string, string> data)
        {
            if(Inited)
                return;
            
#if UNITY_IOS
            //включаем отслеживание рекламы через SkAdNetwork для AudienceNetwork
            if (Game.User.AudienceTrackingEnabled)
                AudienceSettings.SetAdvertiserTrackingEnabled(Game.User.AudienceTrackingEnabled);
#endif
            
            if (data == null)
            {
                Debug.LogError(TAG + "Advert params not set");
                return;
            }
            
            Inited = true;
            
            if (data.ContainsKey(GA_RWRD))
                _adMobRewardId = data[GA_RWRD];

            if (data.ContainsKey(GA_INT))
                _adMobInterstitialId = data[GA_INT];

            MobileAds.Initialize(initStatus =>
            {
                Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
                foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
                {
                    string className = keyValuePair.Key;
                    AdapterStatus status = keyValuePair.Value;
                    switch (status.InitializationState)
                    {
                        case AdapterState.NotReady:
                            Debug.Log(TAG + "Adapter: " + className + " not ready.");
                            break;
                        case AdapterState.Ready:
                            Debug.Log(TAG + "Adapter: " + className + " is initialized.");
                            break;
                    }
                }
                
                OnMobileAdsInitialized();
            });
        }

        private void OnMobileAdsInitialized()
        {
            if (!string.IsNullOrEmpty(_adMobRewardId))
            {
                _rewardAd = new RewardedAd(_adMobRewardId);
                SubscribeRewardAd();
                LoadRewardAd();
            }

            if (!string.IsNullOrEmpty(_adMobInterstitialId))
            {
                _interstitial = new InterstitialAd(_adMobInterstitialId);
                SubscribeInterstitial();
                LoadInterstitialAd();
            }
        }

        protected override void LoadInterstitialAd()
        {
            base.LoadInterstitialAd();
            _interstitial.LoadAd(AdRequest);
        }

        protected override void LoadRewardAd()
        {
            base.LoadRewardAd();
            _rewardAd.LoadAd(AdRequest);
        }

        protected override void StartShowInterstitial()
        {
            _interstitial.Show();
        }

        protected override void StartShowReward()
        {
            _rewardAd.Show();
        }

        private void SubscribeInterstitial()
        {
            _interstitial.OnAdLoaded += HandleOnInterstitialAdLoaded;
            _interstitial.OnAdFailedToLoad += HandleOnInterstitialAdFailedToLoad;
            _interstitial.OnAdOpening += HandleOnInterstitialAdOpened;
            _interstitial.OnAdClosed += HandleOnInterstitialAdClosed;
        }
        
        private void SubscribeRewardAd()
        {
            _rewardAd.OnAdLoaded += HandleRewardBasedVideoLoaded;
            _rewardAd.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
            _rewardAd.OnAdOpening += HandleRewardBasedVideoOpened;
            _rewardAd.OnUserEarnedReward += HandleRewardBasedVideoRewarded;
            _rewardAd.OnAdClosed += HandleRewardBasedVideoClosed;
        }

        private void HandleOnInterstitialAdLoaded(object sender, EventArgs args) => OnInterstitialLoaded();
        private void HandleOnInterstitialAdOpened(object sender, EventArgs args) => OnInterstitialOpened();
        private void HandleOnInterstitialAdClosed(object sender, EventArgs args) => OnInterstitialClosed();

        private void HandleRewardBasedVideoLoaded(object sender, EventArgs args) => OnRewardLoaded();
        private void HandleRewardBasedVideoOpened(object sender, EventArgs args) => OnRewardOpened(); 
        private void HandleRewardBasedVideoClosed(object sender, EventArgs args) => OnRewardClosed();
        private void HandleRewardBasedVideoRewarded(object sender, Reward args) => OnRewarded(args.Amount + " " + args.Type);
        
        private void HandleOnInterstitialAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            OnInterstitialLoadFailed($"[{args.LoadAdError.GetCode()}] {args.LoadAdError.GetMessage()}");
            
            Debug.Log("Load error string: " + args.LoadAdError);
            Debug.Log("Response info: " + args.LoadAdError.GetResponseInfo());
        }

        private void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            OnRewardLoadFailed($"[{args.LoadAdError.GetCode()}] {args.LoadAdError.GetMessage()}");
            
            Debug.Log("Load error string: " + args.LoadAdError);
            Debug.Log("Response info: " + args.LoadAdError.GetResponseInfo());
        }
    }
}
