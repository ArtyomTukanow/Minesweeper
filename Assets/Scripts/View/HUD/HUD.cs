using System.Collections.Generic;
using AllUtils;
using DG.Tweening;
using UnityEngine;
using View.Window;

namespace View.HUD
{
    public class HUD : MonoBehaviour
    {
        public const float HUD_HIDE_TIME = .5f;
        private const string HIDE_DEF_KEY = "def";
        private const string HIDE_WND_KEY = "windows";
        
        private readonly List<string> hideKeys = new List<string>();
        private bool isShowed = true;
        private Tween hudTween;
        
        public HudContent Content { get; private set; }

        public void HudInit()
        {
            Content = Instantiate(BasePrefabs.Instance.content, transform);
        }
        
        
        private void Awake()
        {
            AddListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            RemoveListeners();
            if (WindowsController.Instance)
            {
                WindowsController.Instance.OnShowWindow += OnWindowEvent;
                WindowsController.Instance.OnHideWindow += OnWindowEvent;
            }

            OnWindowEvent(null);
        }

        private void RemoveListeners()
        {
            if (WindowsController.Instance)
            {
                WindowsController.Instance.OnShowWindow -= OnWindowEvent;
                WindowsController.Instance.OnHideWindow -= OnWindowEvent;
            }
        }
        
        private void OnWindowEvent(IWindow wnd)
        {
            if(WindowsController.Instance && WindowsController.Instance.ActiveWindowsCount > 0)
                Hide(HIDE_WND_KEY);
            else
                Show(HIDE_WND_KEY);
        }

        public void Show(string key = HIDE_DEF_KEY)
        {
            hideKeys.Remove(key);
            UpdateVisible();
        }

        public void Hide(string key = HIDE_DEF_KEY)
        {
            hideKeys.AddOnce(key);
            UpdateVisible();
        }

        private void UpdateVisible()
        {
            var needShow = hideKeys.Count == 0;
            if (isShowed != needShow)
            {
                isShowed = needShow;
                
                if(needShow)
                    gameObject.SetActive(true);
                
                hudTween?.Kill();
                hudTween = TweenHudAlpha(isShowed ? 1 : 0)
                    .OnComplete(() =>
                    {
                        if(gameObject && !needShow)
                            gameObject.SetActive(false);
                    });
            }
        }

        private Tween TweenHudAlpha(float endVal)
        {
            Content.canvasGroup.interactable = endVal >= 1;

            return DOTween.To(
                () => Content.canvasGroup.alpha,
                x => Content.canvasGroup.alpha = x,
                endVal,
                HUD_HIDE_TIME);
        }
    }
}