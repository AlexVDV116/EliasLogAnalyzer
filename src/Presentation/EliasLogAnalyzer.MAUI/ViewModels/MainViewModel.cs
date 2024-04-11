using System.Collections.ObjectModel;
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
        public IRelayCommand LoadLogfilesCommand { get; }
        
        public ObservableCollection<string> SelectedFileNames { get; } = new ObservableCollection<string>();


        public MainViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            ChangeToDarkThemeCommand = new RelayCommand(() => ApplyTheme(Theme.Dark));
            ChangeToLightThemeCommand = new RelayCommand(() => ApplyTheme(Theme.Light));
            ChangeToSystemThemeCommand = new RelayCommand(() => ApplyTheme(Theme.System));
            LoadLogfilesCommand = new RelayCommand(LoadLogFiles);
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
        
        private async void LoadLogFiles()
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new List<string> { ".log" } },
                { DevicePlatform.MacCatalyst , new List<string> { "public.log" } }
            });
            
            var results = await FilePicker.PickMultipleAsync(new PickOptions
            {
                FileTypes = customFileType
            });

            foreach (var result in results)
            {
                App.Current.Dispatcher.Dispatch(() =>
                {
                    SelectedFileNames.Add(result.FileName);
                });
            }
        }
    }
}
