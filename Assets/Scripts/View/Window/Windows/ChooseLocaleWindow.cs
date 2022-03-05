using AllUtils;
using Core;
using UnityEngine;
using View.Window.Settings;
using View.Window.Views;

namespace View.Window.Windows
{
    public class ChooseLocaleWindow : Window<ChooseLocaleView>
    {
        public override string PathOfView => "Windows/ChooseLocaleWindow";

        public ChooseLocaleWindow(WindowSettings settings = null) : base(settings)
        {
        }
        
        public static void Of()
        {
            var settings = new WindowSettings.Builder
            {
                ShowInQueue = false
            }.Build();
            
            var wnd = new ChooseLocaleWindow(settings);
            WindowsController.Instance.AddWindow(wnd);
        }
        
        protected override void OnShow()
        {
            base.OnShow();

            View.Title.text = "choose_language".Localize();
            
            View.RusButton.Init(OnLocalizeClick);
            View.EngButton.Init(OnLocalizeClick);
        }

        private void OnLocalizeClick(SystemLanguage language)
        {
            Hide();
            Game.User.Settings.Language = language;
        }
    }
}