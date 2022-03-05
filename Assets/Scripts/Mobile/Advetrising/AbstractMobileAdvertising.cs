using System;
using System.Collections.Generic;
using Core;
using DG.Tweening;
using Libraries.RSG;
using UniRx;
using UnityEngine;

namespace Mobile.Advetrising
{
    
    /// <summary>
    /// Класс упрвляет загрузкой, показом и логированием рекламы на различных платформах.
    /// </summary>
    public abstract class AbstractMobileAdvertising : MonoBehaviour
    {
        protected const string INTERSTITIAL = "INTERSTITIAL";
        protected const string REWARD = "REWARD";

        private const string LOADING_ERROR_TIMEOUT = "err_timeout";
        private const string LOADING_SUCCESS_TIMEOUT = "succ_timeout";

        private int _loadingErrorTimeout = 15;
        private int _loadingSuccessTimeout = 2;

        protected string TAG => $"[ADS][{Name.ToUpper()}] ";

        protected abstract string Name { get; }
        
        public virtual ReactiveProperty<bool> IsRewardLoadedReactive { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsInterstitialLoadedReactive { get; } = new ReactiveProperty<bool>();

        public bool IsAdShowing { get; private set; } = false;
        public bool Inited { get; protected set; }

        public abstract void UpdateData(Dictionary<string, string> data);

        public void UpdateServerSettings(Dictionary<string, string> data)
        {
            if(data == null)
                return;

            if (data.ContainsKey(LOADING_ERROR_TIMEOUT))
                _loadingErrorTimeout = int.Parse(data[LOADING_ERROR_TIMEOUT]);

            if (data.ContainsKey(LOADING_SUCCESS_TIMEOUT))
                _loadingSuccessTimeout = int.Parse(data[LOADING_SUCCESS_TIMEOUT]);
        }





        #region interstitial

        protected abstract void StartShowInterstitial();
        
        protected virtual void LoadInterstitialAd()
        {
            Game.ExecuteOnMainThread(() => Debug.Log(TAG + "Interstitial start to load"));
            IsInterstitialLoadedReactive.Value = false;
        }

        public Promise<bool> ShowInterstitial()
        {
            var result = new Promise<bool>();
            
            if (IsInterstitialLoadedReactive.Value)
            {
                IsAdShowing = true;
                
                StartShowInterstitial();
                result.Resolve(true);
            }
            else
            {
                result.Resolve(false);
            }

            return result;
        }

        protected void OnInterstitialLoaded()
        {
            IsInterstitialLoadedReactive.Value = true;
            Game.ExecuteOnMainThread(() => Debug.Log(TAG + "Interstitial Loaded"));
        }

        protected virtual void OnInterstitialLoadFailed(string errMsg = "")
        {
            IsInterstitialLoadedReactive.Value = false;
            Game.ExecuteOnMainThread(() => Debug.Log(TAG + "Interstitial Load Failed : " + errMsg));

            if(_loadingErrorTimeout >= 0)
                DOVirtual.DelayedCall(_loadingErrorTimeout, LoadInterstitialAd);
        }

        protected void OnInterstitialOpened()
        {
            Game.ExecuteOnMainThread(() => Debug.Log(TAG + "Interstitial Opened"));
        }

        protected void OnInterstitialClosed()
        {
            IsInterstitialLoadedReactive.Value = false;
            
            IsInterstitialLoadedReactive.Value = false;
            IsAdShowing = false;
            
            DOVirtual.DelayedCall(_loadingSuccessTimeout, LoadInterstitialAd);
        }

        #endregion





        #region reward

        private Action _onCompleteReward;
        private Action _onCloseReward;


        protected abstract void StartShowReward();

        protected virtual void LoadRewardAd()
        {
            Game.ExecuteOnMainThread(() => Debug.Log(TAG + "Reward start to load"));
            IsRewardLoadedReactive.Value = false;
        }

        protected void OnRewardComplete()
        {
            IsAdShowing = false;
            Game.ExecuteOnMainThread(() =>Debug.Log(TAG + "Reward Completed"));
            // EventController.TriggerEvent(new GameEvents.UserAdsWatchStatusUpdate(GameEvents.UserAdsWatchStatusUpdate.AdUpdateStatus.Successed));
        }

        protected void OnRewardLoaded()
        {
            Game.ExecuteOnMainThread(
                () =>
                {
                    IsRewardLoadedReactive.Value = true;
                    Debug.Log(TAG + "Reward Loaded");
                    // EventController.TriggerEvent(new GameEvents.UserAdsWatchStatusUpdate());
                });
        }

        protected virtual void OnRewardLoadFailed(string errMsg = "")
        {
            IsRewardLoadedReactive.Value = false;
            Game.ExecuteOnMainThread(() => Debug.Log(TAG + "Reward Load failed: " + errMsg));
            
            IsRewardLoadedReactive.Value = false;
            _onCloseReward?.Invoke();
            _onCloseReward = null;

            if(_loadingErrorTimeout >= 0)
                DOVirtual.DelayedCall(_loadingErrorTimeout, LoadRewardAd);
        }

        protected void OnRewardOpened()
        {
            Game.ExecuteOnMainThread(() =>Debug.Log(TAG + "Reward opened"));
        }

        protected void OnRewardClosed()
        {
            IsRewardLoadedReactive.Value = false;
            
            IsAdShowing = false;
            
            Game.ExecuteOnMainThread(
                () =>
                {
                    Debug.Log(TAG + "Reward closed");
                    // EventController.TriggerEvent(new GameEvents.UserAdsWatchStatusUpdate(GameEvents.UserAdsWatchStatusUpdate.AdUpdateStatus.Closed));
                });

            if(_loadingSuccessTimeout >= 0)
                DOVirtual.DelayedCall(_loadingSuccessTimeout, LoadRewardAd);

            IsRewardLoadedReactive.Value = false;
            _onCloseReward?.Invoke();
            _onCloseReward = null;
        }

        protected void OnRewarded(string msg = "")
        {
            Game.ExecuteOnMainThread(
                () =>
                {
                    Debug.Log(TAG + "Reward rewarded " + msg);

                        _onCompleteReward?.Invoke();
                    _onCompleteReward = null;
                });
        }

        public bool ShowRewardAd(Action onComplete, Action onClose)
        {
            if (IsRewardLoadedReactive.Value)
            {
                Debug.Log(TAG + "ShowRewardAd");

                IsAdShowing = true;
                
                _onCompleteReward = onComplete;
                _onCloseReward = onClose;

                StartShowReward();
                // ShowTestReward();
                
                return true;
            }

            Debug.Log(TAG + "ShowRewardAd Not Loaded");

            return false;
        }

        #endregion
    }
}