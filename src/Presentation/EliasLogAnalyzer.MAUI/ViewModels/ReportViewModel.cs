using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class ReportViewModel : ObservableObject
{
    private readonly ILogDataSharingService _logDataSharingService;
    private readonly IDialogService _dialogService;
    private readonly IApiService _apiService;

    [ObservableProperty] private bool _isConnected;
    [ObservableProperty] private bool _isChecking;
    [ObservableProperty] private bool _connectedIconVisible;
    [ObservableProperty] private bool _notConnectedIconVisible;
    [ObservableProperty] private string _connectionStatus = "Checking connection...";

    [ObservableProperty] private DateTime _reportDate = DateTime.Now;
    [ObservableProperty] private TimeSpan _reportTime = DateTime.Now.TimeOfDay;
    [ObservableProperty] private string _analysis = string.Empty;
    [ObservableProperty] private string _possibleSolutions = string.Empty;
    [ObservableProperty] private string _risk = string.Empty;
    [ObservableProperty] private string _impact = string.Empty;
    [ObservableProperty] private string _recommendation = string.Empty;
    [ObservableProperty] private string _emptyCollectionViewText = "Pin LogEntries to add them to the report.";
    [ObservableProperty] private bool _showPinImage;
    
    [ObservableProperty] private Color _analysisPlaceholderColor = Colors.LightGray;
    [ObservableProperty] private Color _recommendationPlaceholderColor = Colors.LightGray;

    [ObservableProperty] private ObservableCollection<LogEntry> _logEntriesToInclude = [];

    // Properties directly bound to the LogDataSharingService which acts as the single source of truth for collections
    private ObservableCollection<LogEntry> LogEntries => _logDataSharingService.LogEntries;
    
    public IAsyncRelayCommand CheckConnectionCommand { get; }
    private DateTime CombinedDateTime => ReportDate.Date + ReportTime;
    
    public ReportViewModel(
        ILogDataSharingService logDataSharingService,
        IDialogService dialogService,
        IApiService apiService)
    {
        _logDataSharingService = logDataSharingService;
        _dialogService = dialogService;
        _apiService = apiService;

        CheckConnectionCommand = new AsyncRelayCommand(CheckDatabaseConnectionAsync);
        
        LogEntries.CollectionChanged += UpdateLogEntriesToInclude;
    }
    
    private void UpdateLogEntriesToInclude(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var filteredAndSortedEntries = _logDataSharingService.LogEntries
            .Where(le => le.IsPinned || le.IsMarked)
            .OrderByDescending(le => le.IsMarked)
            .ThenBy(le => le.LogTimeStamp)
            .ToList();

        LogEntriesToInclude.Clear();
        foreach (var entry in filteredAndSortedEntries)
        {
            LogEntriesToInclude.Add(entry);
        }
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
            ConnectionStatus = string.IsNullOrEmpty(result.ErrorMessage) ? "Not connected" : result.ErrorMessage;
        }
    }

    [RelayCommand]
    private async Task CheckDatabaseConnectionAsync()
    {
        IsChecking = true;
        var result = await _apiService.CheckDatabaseConnectionAsync();
        UpdateConnectionStatus(result);
        IsChecking = false;
    }

    [RelayCommand]
    private async Task CopyToClipboardAsync()
    {
        // Generate a string for each pinned log entry, displaying critical information
        var pinnedLogEntriesDetails = string.Join("\n\n", LogEntriesToInclude.Select(entry =>
            $"DateTime: {entry.LogTimeStamp.DateTime} (Ticks: {entry.LogTimeStamp.Ticks}), " +
            $"Type: {entry.LogType}, " +
            $"Source: {entry.Source}, " +
            $"User: {entry.User}, " +
            $"Computer: {entry.Computer}\n" +
            $"Description: {entry.Description}\n" +
            $"Data:\n{entry.Data}"));

        var reportDetails = $"Elias LogAnalyzer Report Details:\n" +
                            $"Date and Time: {CombinedDateTime}\n" +
                            $"Analysis: {Analysis}\n" +
                            $"Possible Solution: {PossibleSolutions}\n" +
                            $"Risk: {Risk}\n" +
                            $"Recommendation: {Recommendation}\n\n" +
                            $"Pinned log entries:\n{pinnedLogEntriesDetails}";

        await Clipboard.SetTextAsync(reportDetails);
        await _dialogService.ShowMessage("Success", "Report details copied to clipboard.");
    }


    [RelayCommand]
    private async Task Submit()
    {
        ValidateForm();
        if (IsFormValid())
        {
            var bugReport = new BugReport
            {
                Analysis = Analysis.Trim(),
                PossibleSolutions = PossibleSolutions.Trim(),
                Risk = Risk.Trim(),
                Recommendation = Recommendation.Trim(),
                LogEntriesToInclude = LogEntriesToInclude.ToList()
            };

            var result = await _apiService.AddBugReportAsync(bugReport);
            if (result.Success)
            {
                await _dialogService.ShowMessage("Success", "Report successfully submitted.");
            }
            else
            {
                await _dialogService.ShowMessage("Error", result.ErrorMessage);
            }
        }
        else
        {
            await _dialogService.ShowMessage("Validation Error", "Please fill in all required fields.");
        }
    }

    private void ValidateForm()
    {
        AnalysisPlaceholderColor = string.IsNullOrWhiteSpace(Analysis) ? Colors.Red : Colors.LightGray;
        RecommendationPlaceholderColor = string.IsNullOrWhiteSpace(Recommendation) ? Colors.Red : Colors.LightGray;

    }

    private bool IsFormValid()
    {
        return !string.IsNullOrWhiteSpace(Analysis) && !string.IsNullOrWhiteSpace(Recommendation);
    }
}