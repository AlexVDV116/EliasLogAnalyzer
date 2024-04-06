using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.MAUI.Resources;
using EliasLogAnalyzer.MAUI.Services;

namespace EliasLogAnalyzer.MAUI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ISettingsService _settingsService;

        public IRelayCommand ChangeToDarkThemeCommand { get; }
        public IRelayCommand ChangeToLightThemeCommand { get; }
        public IRelayCommand ChangeToSystemThemeCommand { get; }

        public MainViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            ChangeToDarkThemeCommand = new RelayCommand(() => ApplyTheme(Theme.Dark));
            ChangeToLightThemeCommand = new RelayCommand(() => ApplyTheme(Theme.Light));
            ChangeToSystemThemeCommand = new RelayCommand(() => ApplyTheme(Theme.System));
        }

        public async Task Initialise()
        {
            // Add code for initialization here
        }

        private void ApplyTheme(Theme theme)
        {
            _settingsService.AppTheme = theme;
            App.Current.Dispatcher.Dispatch(() =>
            {
                App.Current.UserAppTheme = theme.AppTheme;
            });
        }
    }
}
