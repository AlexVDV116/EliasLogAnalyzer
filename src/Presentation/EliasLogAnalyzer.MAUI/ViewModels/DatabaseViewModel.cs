using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using System.Diagnostics;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class DatabaseViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty] private bool _isConnected;
    [ObservableProperty] private bool _isChecking;
    [ObservableProperty] private bool _connectedIconVisible;
    [ObservableProperty] private bool _notConnectedIconVisible;
    [ObservableProperty] private string _connectionStatus = "Checking connection...";

    public IAsyncRelayCommand CheckConnectionCommand { get; }

    public DatabaseViewModel(IApiService apiService)
    {
        _apiService = apiService;
        CheckConnectionCommand = new AsyncRelayCommand(CheckDatabaseConnectionAsync);
    }

    private void UpdateConnectionStatus(ApiResult result)
    {
        if (result.Success)
        {
            IsConnected = true;
            NotConnectedIconVisible = false;
            ConnectedIconVisible = true;
            ConnectionStatus = "Connected";
        }
        else
        {
            IsConnected = false;
            ConnectedIconVisible = false;
            NotConnectedIconVisible = true;
            ConnectionStatus = result.ErrorMessage;
        }
    }

    [RelayCommand]
    private async Task CheckDatabaseConnectionAsync()
    {
        IsChecking = true;
        Debug.WriteLine("Checking database connection...");
        var result = await _apiService.CheckDatabaseConnectionAsync();
        UpdateConnectionStatus(result);
        IsChecking = false;
    }
}