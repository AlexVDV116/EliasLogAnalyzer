using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI
{
    public partial class App : Application
    {
        private readonly ISettingsService _settingsService;

        public App(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            
            InitializeComponent();
            MainPage = new AppShell(settingsService);
            SetTheme();
        }

        private void SetTheme()
        {
            UserAppTheme = _settingsService.AppTheme != null
                         ? _settingsService.AppTheme.AppTheme
                         : AppTheme.Unspecified;
        }
        
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

            const int newWidth = 1920;
            const int newHeight = 1080;
        
            window.Width = newWidth;
            window.Height = newHeight;
            
            window.MinimumHeight = newHeight;
            window.MinimumWidth = newWidth;

            return window;
        }
    }
}
