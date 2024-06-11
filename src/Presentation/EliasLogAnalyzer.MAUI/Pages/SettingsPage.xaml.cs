using EliasLogAnalyzer.MAUI.ViewModels;
using Microsoft.Maui.Controls;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel settingsViewModel)
    {
        InitializeComponent();

        BindingContext = settingsViewModel;
    }
}