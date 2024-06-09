using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using EliasLogAnalyzer.MAUI.Resources;


namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IApiService _apiService;

    [ObservableProperty] private bool _isConnected;
    [ObservableProperty] private bool _connectedIconVisible;
    [ObservableProperty] private bool _notConnectedIconVisible;
    [ObservableProperty] private bool _toggleHeaderTextVisibility;
    [ObservableProperty] private double _flyoutWidth = 80;
    [ObservableProperty] private string _connectionStatus = "Checking connection...";

    public IAsyncRelayCommand CheckConnectionCommand { get; }

    public AppShellViewModel(ISettingsService settingsService, IApiService apiService)
    {
        _settingsService = settingsService;
        _apiService = apiService;
        CheckConnectionCommand = new AsyncRelayCommand(CheckConnectionStatusAsync);
    }

    [RelayCommand]
    private async Task CheckConnectionStatusAsync()
    {
        try
        {
            var result = await _apiService.CheckDatabaseConnectionAsync();
            IsConnected = result.Success;
            ConnectionStatus = result.Success ? "Connected" : "Not connected";
            ConnectedIconVisible = result.Success;
            NotConnectedIconVisible = !result.Success;
        }
        catch (Exception)
        {
            ConnectionStatus = "Failed to check connection";
            IsConnected = false;
            ConnectedIconVisible = false;
            NotConnectedIconVisible = true;
        }
    }
    [RelayCommand]
    private void ToggleFlyoutWidth()
    {
        FlyoutWidth = FlyoutWidth == 80 ? 160 : 80;
        ToggleHeaderTextVisibility = !ToggleHeaderTextVisibility;
    }

    [RelayCommand]
    private void ApplyTheme(Theme theme)
    {
        _settingsService.AppTheme = theme;
        Application.Current?.Dispatcher.Dispatch(() => { Application.Current.UserAppTheme = theme.AppTheme; });
    }
}

