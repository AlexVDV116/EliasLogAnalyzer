using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel settingsViewModel)
    {
        InitializeComponent();

        BindingContext = settingsViewModel;
    }
}