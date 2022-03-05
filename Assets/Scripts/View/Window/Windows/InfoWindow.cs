using System;
using AllUtils;
using View.Window.Settings;
using View.Window.Views;

namespace View.Window.Windows
{
    public class InfoWindow: Window<InfoView>
    {
        private string pathOfView = "Windows/Info";
        public override string PathOfView => pathOfView;

        private string title;
        private string description;

        private Action onHide;
        private Action onOkClick;
        private Action onCancelClick;

        private string yes = "yes".Localize();
        private string no = "no".Localize();
        private string ok = "ok".Localize();

        private bool isYesNoType = false;
        
        public InfoWindow(WindowSettings settings) : base(settings)
        {
            
        }

        public static InfoWindow Of(bool inQueue = false)
        {
            var settings = new WindowSettings.Builder
            {
                ShowInQueue = inQueue
            }.Build();
            
            return Of(settings);
        }

        public static InfoWindow Of(WindowSettings settings) => new InfoWindow(settings);

        public InfoWindow SetPathOfView(string value)
        {
            pathOfView = value;
            return this;
        }

        public InfoWindow SetDescription(string value)
        {
            description = value;
            return this;
        }
        
        public InfoWindow SetTitle(string value)
        {
            title = value;
            return this;
        }
        
        public InfoWindow SetOnOk(Action value)
        {
            onOkClick = value;
            return this;
        }
        
        public InfoWindow SetOnCancel(Action value)
        {
            onCancelClick = value;
            return this;
        }
        
        public InfoWindow SetOnHide(Action value)
        {
            onHide = value;
            return this;
        }

        public InfoWindow SetYesNoType(string yes, string no)
        {
            isYesNoType = true;
            this.yes = yes;
            this.no = no;

            return this;
        }

        public InfoWindow SetOkType(string ok)
        {
            isYesNoType = false;
            this.ok = ok;

            return this;
        }

        public void Build()
        {
            WindowsController.Instance.AddWindow(this);
        }

        protected override void OnShow()
        {
            base.OnShow();

            View.description.gameObject.SetActive(!description.IsNullOrEmpty());
            View.description.text = description;
            
            View.title.gameObject.SetActive(!title.IsNullOrEmpty());
            View.title.text = title;

            View.okText.text = ok;
            View.yesText.text = yes;
            View.noText.text = no;

            View.okButton.gameObject.SetActive(!isYesNoType);
            View.yesButton.gameObject.SetActive(isYesNoType);
            View.noButton.gameObject.SetActive(isYesNoType);
            
            View.okButton.onClick.AddListener(OnOkClick);
            View.yesButton.onClick.AddListener(OnOkClick);
            View.noButton.onClick.AddListener(OnCancelClick);
        }

        private void OnOkClick()
        {
            onOkClick?.Invoke();
            Hide();
        }

        private void OnCancelClick()
        {
            onCancelClick?.Invoke();
            Hide();
        }

        protected override void OnHide()
        {
            base.OnHide();
            onHide?.Invoke();
        }
    }
}