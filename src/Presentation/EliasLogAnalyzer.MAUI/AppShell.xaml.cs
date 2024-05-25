using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.MAUI.Pages;
using EliasLogAnalyzer.MAUI.Resources;
using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI
{
    public partial class AppShell : Shell
    {
        private readonly ISettingsService _settingsService;

        public IRelayCommand ChangeToDarkThemeCommand { get; }
        public IRelayCommand ChangeToLightThemeCommand { get; }
        public IRelayCommand ChangeToSystemThemeCommand { get; }

        public AppShell(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            InitializeComponent();
            Routing.RegisterRoute("logentriesPage", typeof(LogEntriesPage));

            this.BindingContext = this;
        }

        private void ApplyTheme(Theme theme)
        {
            _settingsService.AppTheme = theme;
            Application.Current?.Dispatcher.Dispatch(() => { Application.Current.UserAppTheme = theme.AppTheme; });
        }
    }
}
