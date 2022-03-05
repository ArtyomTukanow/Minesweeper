namespace View.Window.Settings
{
    public class WindowSettings
    {
        private const int DEF_PRIORY = 100;
        
        public int Priory { get; private set; } = DEF_PRIORY;
        public bool ShowInQueue { get; private set; } = true;
        public bool EscSensitive { get; private set; } = true;
        public bool ShowCellsText { get; private set; } = false;
        public bool PauseGameOnShow { get; private set; } = true;

        private WindowSettings()
        {
            
        }
        
        public class Builder
        {
            private WindowSettings settings = new WindowSettings();
            
            public int Priory 
            { 
                get => settings.Priory;
                set => settings.Priory = value;
            }
            
            public bool ShowInQueue 
            { 
                get => settings.ShowInQueue;
                set => settings.ShowInQueue = value;
            }
            
            public bool EscSensitive 
            { 
                get => settings.EscSensitive;
                set => settings.EscSensitive = value;
            }
            
            public bool ShowCellsText 
            { 
                get => settings.ShowCellsText;
                set => settings.ShowCellsText = value;
            }
            
            public bool PauseGameOnShow 
            { 
                get => settings.PauseGameOnShow;
                set => settings.PauseGameOnShow = value;
            }
            
            public WindowSettings Build() => settings;
        }
    }
}