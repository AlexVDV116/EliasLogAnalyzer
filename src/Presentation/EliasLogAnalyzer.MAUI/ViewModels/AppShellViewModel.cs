using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using EliasLogAnalyzer.MAUI.Resources;
using Microsoft.Maui.Controls;


namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class AppShellViewModel(ISettingsService settingsService) : ObservableObject
{
    [ObservableProperty] private bool _toggleHeaderTextVisibility;
    [ObservableProperty] private double _flyoutWidth = 80;

    [RelayCommand]
    private void ToggleFlyoutWidth()
    {
        FlyoutWidth = FlyoutWidth == 80 ? 160 : 80;
        ToggleHeaderTextVisibility = !ToggleHeaderTextVisibility;
    }

    [RelayCommand]
    private void ApplyTheme(Theme theme)
    {
        settingsService.AppTheme = theme;
        Application.Current?.Dispatcher.Dispatch(() => { Application.Current.UserAppTheme = theme.AppTheme; });
    }
}

