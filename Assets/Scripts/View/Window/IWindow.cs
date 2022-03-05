using System;
using Libraries.RSG;
using View.Window.Settings;

namespace View.Window
{
    public interface IWindow
    {
        public WindowSettings Settings { get; }
        public string PathOfView { get; }
        public IPromise Preload();
        public IPromise Show();
        public IPromise Hide();
        void SetController(WindowsController controller, Action<IWindow> OnShow, Action<IWindow> OnHide);
    }
}