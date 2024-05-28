﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Microsoft.Extensions.Logging;
using EliasLogAnalyzer.MAUI.Resources;

namespace EliasLogAnalyzer.MAUI.ViewModels
{
    public partial class AppShellViewModel : ObservableObject
    {
        private readonly ISettingsService _settingsService;
        [ObservableProperty] private double _flyoutWidth = 160;
        [ObservableProperty] private bool _toggleHeaderTextVisibility = true;

        public AppShellViewModel(
            ILogger<AppShellViewModel> logger,
            ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [RelayCommand]
        public void ToggleFlyoutWidth()
        {
            FlyoutWidth = FlyoutWidth == 160 ? 80 : 160;
            ToggleHeaderTextVisibility = !ToggleHeaderTextVisibility;

        }


        [RelayCommand]
        private void ApplyTheme(Theme theme)
        {
            _settingsService.AppTheme = theme;
            Application.Current?.Dispatcher.Dispatch(() => { Application.Current.UserAppTheme = theme.AppTheme; });
        }
    }
}
