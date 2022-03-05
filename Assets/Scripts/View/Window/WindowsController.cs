using System;
using System.Collections.Generic;
using System.Linq;
using AllUtils;
using Controller.Map;
using UnityEngine;
using UnityEngine.UI;
using View.Window.Windows;

namespace View.Window
{
    public class WindowsController : MonoBehaviour
    {
        public CanvasScaler canvasScaler;
        
        public readonly List<IWindow> WindowsQueue = new List<IWindow>();
        public readonly List<IWindow> ActiveWindows = new List<IWindow>();
        
        private static WindowsController instance;
        public static WindowsController Instance => instance;

        public int ActiveWindowsCount => ActiveWindows.Count;

        public event Action<IWindow> OnShowWindow;
        public event Action<IWindow> OnHideWindow;
        public event Action OnWindowEvent;

        private void Awake()
        {
            instance = this;
        }

        public void SetMatch(float match)
        {
            canvasScaler.matchWidthOrHeight = match;
        }

        public void AddWindow(IWindow window)
        {
            window.SetController(this, OnShow, OnHide);
            window.Preload();
            if (window.Settings.ShowInQueue)
            {
                WindowsQueue.Add(window);
                TryShowNextWindow();
            }
            else
            {
                window.Show();
            }
        }

        private void OnShow(IWindow window)
        {
            WindowsQueue.Remove(window);
            ActiveWindows.AddOnce(window);
            
            if(window.Settings.PauseGameOnShow)
                MapController.Instance.UserMap.TryUpdateState();
            
            OnShowWindow?.Invoke(window);
            OnWindowEvent?.Invoke();
        }

        private void OnHide(IWindow window)
        {
            ActiveWindows.Remove(window);
            TryShowNextWindow();
            
            if(window.Settings.PauseGameOnShow)
                MapController.Instance.UserMap.TryUpdateState();
            
            OnHideWindow?.Invoke(window);
            OnWindowEvent?.Invoke();
        }

        private void TryShowNextWindow()
        {
            if(ActiveWindows.Count > 0)
                return;
            
            IWindow wnd = null;
            var priory = int.MinValue;

            foreach (var window in WindowsQueue)
            {
                if (priory < window.Settings.Priory)
                {
                    priory = window.Settings.Priory;
                    wnd = window;
                }
            }

            wnd?.Show();
        }

        public T GetCurrentWindow<T>() where T: IWindow
        {
            return (T)ActiveWindows.FirstOrDefault(w => w is T);
        }
        
        
        private void Update()
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE || UNITY_WSA
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GetCurrentWindow<IWindow>() is {} window)
                {
                    if(window.Settings.EscSensitive)
                        window.Hide();
                }
                else 
                    MainMenuWindow.Of();
            }
#endif
        }
    }
}