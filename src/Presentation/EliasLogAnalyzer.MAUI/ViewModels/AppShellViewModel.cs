using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Microsoft.Extensions.Logging;
using EliasLogAnalyzer.MAUI.Resources;

namespace EliasLogAnalyzer.MAUI.ViewModels
{
    public partial class AppShellViewModel : ObservableObject
    {
        private readonly ISettingsService _settingsService;

        public AppShellViewModel(
            ILogger<AppShellViewModel> logger,
            ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [RelayCommand]
        private void ApplyTheme(Theme theme)
        {
            _settingsService.AppTheme = theme;
            Application.Current?.Dispatcher.Dispatch(() => { Application.Current.UserAppTheme = theme.AppTheme; });
        }
    }
}

