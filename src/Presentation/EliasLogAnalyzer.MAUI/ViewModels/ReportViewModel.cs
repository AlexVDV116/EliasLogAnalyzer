using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Graphics;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class ReportViewModel : ObservableObject
{
    private readonly ILogDataSharingService _logDataSharingService;
    private readonly IDialogService _dialogService;
    private readonly IApiService _apiService;

    [ObservableProperty] private string _developerName = string.Empty;
    [ObservableProperty] private string _workstationName = string.Empty;
    [ObservableProperty] private DateTime _reportDate = DateTime.Now;
    [ObservableProperty] private TimeSpan _reportTime = DateTime.Now.TimeOfDay;
    [ObservableProperty] private string _situation = string.Empty;
    [ObservableProperty] private string _observation = string.Empty;
    [ObservableProperty] private string _expectation = string.Empty;
    [ObservableProperty] private string _tag = string.Empty;
    [ObservableProperty] private string _build = string.Empty;
    [ObservableProperty] private string _severity = string.Empty;
    [ObservableProperty] private string _analysis = string.Empty;
    [ObservableProperty] private string _possibleSolutions = string.Empty;
    [ObservableProperty] private string _whatToTest = string.Empty;
    [ObservableProperty] private double _effort;
    [ObservableProperty] private string _risk = string.Empty;
    [ObservableProperty] private string _workaround = string.Empty;
    [ObservableProperty] private string _recommendation = string.Empty;
    [ObservableProperty] private string _pinnedLogEntriesText = string.Empty;
    [ObservableProperty] private bool _showPinImage;

    [ObservableProperty] private Color _developerNamePlaceholderColor = Colors.LightGray;
    [ObservableProperty] private Color _analysisPlaceholderColor = Colors.LightGray;
    [ObservableProperty] private Color _recommendationPlaceholderColor = Colors.LightGray;
    [ObservableProperty] private Color _severityTextColor = Colors.LightGray;

    // Properties directly bound to the LogDataSharingService which acts as the single source of truth for collections
    private ObservableCollection<LogEntry> PinnedLogEntries => _logDataSharingService.PinnedLogEntries;
    
    private DateTime CombinedDateTime => ReportDate.Date + ReportTime;
    public List<string> Severities { get; } = ["Critical", "High", "Medium", "Low"];
    public List<double> Efforts { get; } = [1, 2, 3, 4, 5, 6, 8, 12, 16, 24];


    public ReportViewModel(
        ILogDataSharingService logDataSharingService,
        IDialogService dialogService,
        IApiService apiService)
    {
        _logDataSharingService = logDataSharingService;
        _dialogService = dialogService;
        _apiService = apiService;

        PinnedLogEntries.CollectionChanged += OnPinnedLogEntriesChanged;
        UpdatePinnedEntriesText();

    }

    private void OnPinnedLogEntriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdatePinnedEntriesText();
    }

    private void UpdatePinnedEntriesText()
    {
        switch (PinnedLogEntries.Count)
        {
            case 0:
                PinnedLogEntriesText = "Pin log entries to include them in the report";
                ShowPinImage = true;
                break;
            case 1:
                PinnedLogEntriesText = "📌 One pinned log entry will be automatically included in the report.";
                ShowPinImage = false;
                break;
            default:
                PinnedLogEntriesText = $"📌 A total of {PinnedLogEntries.Count} pinned log entries will be automatically included in the report.";
                ShowPinImage = false;
                break;
        }
    }

    [RelayCommand]
    private async Task CopyToClipboardAsync()
    {
        // Generate a string for each pinned log entry, displaying critical information
        var pinnedLogEntriesDetails = string.Join("\n\n", PinnedLogEntries.Select(entry =>
            $"DateTime: {entry.LogTimeStamp.DateTime} (Ticks: {entry.LogTimeStamp.Ticks}), " +
            $"Type: {entry.LogType}, " +
            $"Source: {entry.Source}, " +
            $"User: {entry.User}, " +
            $"Computer: {entry.Computer}\n" +
            $"Description: {entry.Description}\n" +
            $"Data:\n{entry.Data}"));

        var reportDetails = $"Elias LogAnalyzer Report Details:\n" +
                            $"Date and Time: {CombinedDateTime}\n" +
                            $"Developer: {DeveloperName}\n" +
                            $"Workstation: {WorkstationName}\n" +
                            $"Severity: {Severity}\n" +
                            $"Situation: {Situation}\n" +
                            $"Observation: {Observation}\n" +
                            $"Expectation: {Expectation}\n" +
                            $"Analysis: {Analysis}\n" +
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
                DeveloperName = DeveloperName.Trim(),
                WorkstationName = WorkstationName.Trim(),
                ReportDateTime = CombinedDateTime,
                Situation = Situation.Trim(),
                Observation = Observation.Trim(),
                Expectation = Expectation.Trim(),
                Tag = Tag.Trim(),
                Build = Build.Trim(),
                Severity = Severity,
                Analysis = Analysis.Trim(),
                PossibleSolutions = PossibleSolutions.Trim(),
                WhatToTest = WhatToTest.Trim(),
                Effort = Effort,
                Risk = Risk.Trim(),
                Workaround = Workaround.Trim(),
                Recommendation = Recommendation.Trim(),
                PinnedLogEntries = _logDataSharingService.PinnedLogEntries.ToList()
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
        DeveloperNamePlaceholderColor = string.IsNullOrWhiteSpace(DeveloperName) ? Colors.Red : Colors.LightGray;
        SeverityTextColor = string.IsNullOrWhiteSpace(Severity) ? Colors.Red : Colors.Black;
        AnalysisPlaceholderColor = string.IsNullOrWhiteSpace(Analysis) ? Colors.Red : Colors.LightGray;
        RecommendationPlaceholderColor = string.IsNullOrWhiteSpace(Recommendation) ? Colors.Red : Colors.LightGray;

    }

    private bool IsFormValid()
    {
        return !string.IsNullOrWhiteSpace(DeveloperName)
               && !string.IsNullOrWhiteSpace(Severity)
               && !string.IsNullOrWhiteSpace(Analysis)
               && !string.IsNullOrWhiteSpace(Recommendation);
    }
}