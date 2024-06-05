using CommunityToolkit.Mvvm.ComponentModel;
using EliasLogAnalyzer.MAUI.Resources;
using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.Services
{
    public partial class SettingsService : ObservableObject, ISettingsService
    {
        [ObservableProperty] private Theme _appTheme;
        
        public SettingsService()
        {
            // set the default appTheme to Light (in future this could be read from the preferences)
            AppTheme = Theme.Light;
        }
    }
}
