using EliasLogAnalyzer.MAUI.Services.Contracts;
using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI
{
    public partial class App : Application
    {
        private readonly ISettingsService _settingsService;
        private readonly LogEntriesViewModel _logEntriesViewModel;
        private readonly AppShellViewModel _appShellViewModel;

        public App(ISettingsService settingsService, AppShellViewModel appShellViewModel, LogEntriesViewModel logEntriesViewModel)
        {
            _settingsService = settingsService;

            InitializeComponent();
            _appShellViewModel = appShellViewModel;
            _logEntriesViewModel = logEntriesViewModel;
            MainPage = new AppShell(_appShellViewModel, _logEntriesViewModel);
            // Setting the theme currently bugs out the flyoutmenu https://github.com/dotnet/maui/issues/22672
            //SetTheme();
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
