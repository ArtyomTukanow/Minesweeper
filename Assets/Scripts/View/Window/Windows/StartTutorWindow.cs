using AllUtils;
using Utils;
using View.Window.Settings;
using View.Window.Views;

namespace View.Window.Windows
{
    public class StartTutorWindow : Window<TutorWindowView>
    {
        public override string PathOfView => "Windows/StartTutor/TutorWindow_" + step;

        private int step;

        public StartTutorWindow(WindowSettings settings) : base(settings)
        {
        }

        public static StartTutorWindow Of(int step)
        {
            var settings = new WindowSettings.Builder
            {
                ShowInQueue = false,
                EscSensitive = false,
                ShowCellsText = true,
                PauseGameOnShow = false,
            }.Build();
            
            var wnd = new StartTutorWindow(settings);
            wnd.step = step;
            
            // WindowsController.Instance.SetMatch(0.5f);
            // wnd.ClosePromise.Then(() => WindowsController.Instance.SetMatch(1f));
            
            WindowsController.Instance.AddWindow(wnd);
            
            return wnd;
        }

        protected override void OnShow()
        {
            base.OnShow();
            DrawText();
            TimerUtils.NextFrame().Then(DrawHole);
        }

        private void DrawText()
        {
            if(View.title)
                View.title.text = $"tutor_{step}_title".Localize();
            if(View.description)
                View.description.text = $"tutor_{step}_desc".Localize();
        }

        private void DrawHole()
        {
            View.softMask.SetParent(View.hole);
        }
    }
}