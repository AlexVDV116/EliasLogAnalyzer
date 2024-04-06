using CommunityToolkit.Mvvm.ComponentModel;
using EliasLogAnalyzer.MAUI.Resources;

namespace EliasLogAnalyzer.MAUI.Services
{
    public partial class SettingsService : ObservableObject, ISettingsService
    {

        public SettingsService()
        {
            // set the default appTheme to Light (in future this could be read from the preferences)
            appTheme = Theme.Light;
        }

        [ObservableProperty]
        public Theme appTheme;

    }
}
