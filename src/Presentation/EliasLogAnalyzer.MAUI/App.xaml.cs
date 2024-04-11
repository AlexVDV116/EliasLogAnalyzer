using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI
{
    public partial class App : Application
    {
        private readonly ISettingsService _settingsService;

        public App(ISettingsService settingsService)
        {
            InitializeComponent();

            MainPage = new AppShell();
            _settingsService = settingsService;
            SetTheme();
        }

        private void SetTheme()
        {
            UserAppTheme = _settingsService.AppTheme != null
                         ? _settingsService.AppTheme.AppTheme
                         : AppTheme.Unspecified;
        }
    }
}
