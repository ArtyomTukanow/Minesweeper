using System;
using AllUtils;
using View.Window.Settings;
using View.Window.Views;

namespace View.Window.Windows
{
    public class RateUsWindow : Window<RateUsWindowView>
    {
        public override string PathOfView => "Windows/RateUsWindow";

        private Action<bool> onClick;

        public RateUsWindow(WindowSettings settings) : base(settings)
        {
        }

        public static RateUsWindow Of(Action<bool> onClick)
        {
            var settings = new WindowSettings.Builder
            {
                ShowInQueue = true,
            }.Build();
            
            var wnd = new RateUsWindow(settings);
            wnd.onClick = onClick;
            WindowsController.Instance.AddWindow(wnd);

            return wnd;
        }

        protected override void OnShow()
        {
            base.OnShow();
            
            View.title.text = "rate_us_title".Localize();
            View.noText.text = "no".Localize();
            View.yesText.text = "yes!".Localize();
            
            View.yesButton.onClick.RemoveAllListeners();
            View.yesButton.onClick.AddListener(() => onClick?.Invoke(true));
            
            View.noButton.onClick.RemoveAllListeners();
            View.noButton.onClick.AddListener(() => onClick?.Invoke(false));
        }
    }
}