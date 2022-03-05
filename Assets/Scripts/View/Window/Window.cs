using System;
using AllUtils;
using Core.AssetsManager;
using Libraries.RSG;
using View.Window.Settings;
using Object = UnityEngine.Object;

namespace View.Window
{
    public abstract class Window<V>: IWindow where V : WindowView
    {
        public WindowsController Controller;
        
        private Action<IWindow> onShow;
        private Action<IWindow> onHide;
        private IPromise preloadOnce;
        
        private Promise closePromise = new Promise();
        public IPromise ClosePromise => closePromise;
        
        
        public V View { get; private set; }
        public WindowSettings Settings { get; }
        
        public abstract string PathOfView { get; }

        
        public Window(WindowSettings settings = null)
        {
            Settings = settings ?? new WindowSettings.Builder().Build();
        }
        
        protected void SetView(V view)
        {
            View = view;
            View.gameObject.SetActive(false);
        }

        public void SetController(WindowsController controller, Action<IWindow> OnShow, Action<IWindow> OnHide)
        {
            Controller = controller;
            onShow = OnShow;
            onHide = OnHide;
        }


        public IPromise Preload()
        {
            ThrowIfNoWindowController();
            
            if (preloadOnce == null)
            {
                var viewLoadPromise = Promise.All(AssetsLoader
                    .CreateAsync<V>(PathOfView, Controller.transform)
                    .Then(SetView)
                    .Then(null));
                preloadOnce = Promise.All(viewLoadPromise, AdditionalPreload());
            }

            return preloadOnce;
        }
        
        
        public IPromise Show()
        {
            ThrowIfNoWindowController();
            onShow?.Invoke(this);
            return Preload().Then(RealShow);
        }

        public IPromise Hide()
        {
            if (closePromise.IsPending)
            {
                OnHide();
                onHide?.Invoke(this);
                Object.Destroy(View.gameObject);
                View = null;
            
                closePromise?.ResolveOnce();
            }

            return Promise.Resolved();
        }
        
        
        
        
        protected virtual IPromise AdditionalPreload() => Promise.Resolved();

        protected virtual void RealShow()
        {
            View.gameObject.SetActive(true);
            DrawByDefault();
            OnShow();
        }
        
        protected virtual void OnShow() {}
        protected virtual void OnHide() {}


        private void DrawByDefault()
        {
            View.backgroundButton?.onClick.RemoveAllListeners();
            View.backgroundButton?.onClick.AddListener(() => Hide());
        }
        
        

        private void ThrowIfNoWindowController()
        {
            if(!Controller)
                throw new Exception("Add WindowController first!");
        }
    }
}