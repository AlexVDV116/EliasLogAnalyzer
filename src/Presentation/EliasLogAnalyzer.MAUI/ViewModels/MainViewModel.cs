using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.MAUI.Resources;
using EliasLogAnalyzer.MAUI.Services;

namespace EliasLogAnalyzer.MAUI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ISettingsService _settingsService;
        private readonly ILogFileLoaderService _logFileLoaderService;

        public IRelayCommand ChangeToDarkThemeCommand { get; }
        public IRelayCommand ChangeToLightThemeCommand { get; }
        public IRelayCommand ChangeToSystemThemeCommand { get; }
        public IRelayCommand LoadLogfilesCommand { get; }
        
        public ObservableCollection<string> LogFiles { get; set; } = [];

        public MainViewModel(ISettingsService settingsService, ILogFileLoaderService logFileLoaderService)
        {
            _settingsService = settingsService;
            _logFileLoaderService = logFileLoaderService;

            ChangeToDarkThemeCommand = new RelayCommand(() => ApplyTheme(Theme.Dark));
            ChangeToLightThemeCommand = new RelayCommand(() => ApplyTheme(Theme.Light));
            ChangeToSystemThemeCommand = new RelayCommand(() => ApplyTheme(Theme.System));
            LoadLogfilesCommand = new RelayCommand(LoadLogFiles);
        }

        //public async Task Initialise()
        //{
        //    // Add code for initialization here
        //}

        private void ApplyTheme(Theme theme)
        {
            _settingsService.AppTheme = theme;
            App.Current.Dispatcher.Dispatch(() =>
            {
                App.Current.UserAppTheme = theme.AppTheme;
            });
        }
        
        private async void LoadLogFiles()
        {
            var results = await _logFileLoaderService.LoadLogFilesAsync();

            if (results != null)
            {
                foreach (var result in results)
                {
                    LogFiles.Add(result.FileName);
                }
            }

        }
    }
}
