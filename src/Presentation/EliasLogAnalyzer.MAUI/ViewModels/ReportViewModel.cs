using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Microsoft.Maui;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class ReportViewModel : ObservableObject
{
    private readonly ILogDataSharingService _logDataSharingService;
    private readonly IDialogService _dialogService;

    [ObservableProperty] private string developerName = string.Empty;
    [ObservableProperty] private string workstationName = string.Empty;
    [ObservableProperty] private DateTime reportDate = DateTime.Now;
    [ObservableProperty] private TimeSpan reportTime = DateTime.Now.TimeOfDay;
    [ObservableProperty] private string situation = string.Empty;
    [ObservableProperty] private string observation = string.Empty;
    [ObservableProperty] private string expectation = string.Empty;
    [ObservableProperty] private string tag = string.Empty;
    [ObservableProperty] private string build = string.Empty;
    [ObservableProperty] private string severity = string.Empty;
    [ObservableProperty] private string analysis = string.Empty;
    [ObservableProperty] private string possibleSolutions = string.Empty;
    [ObservableProperty] private string whatToTest = string.Empty;
    [ObservableProperty] private double effort = 0;
    [ObservableProperty] private string risk = string.Empty;
    [ObservableProperty] private string workaround = string.Empty;
    [ObservableProperty] private string recommendation = string.Empty;
    [ObservableProperty] private string _pinnedLogEntriesText = string.Empty;
    [ObservableProperty] private bool _showPinImage;

    [ObservableProperty] private Color _developerNamePlaceholderColor = Colors.LightGray;
    [ObservableProperty] private Color _analysisPlaceholderColor = Colors.LightGray;
    [ObservableProperty] private Color _recomendationPlaceholderColor = Colors.LightGray;
    [ObservableProperty] private Color _severityTextColor = Colors.LightGray;

    // Properties directly bound to the LogDataSharingService which acts as the single source of truth for collections
    public ObservableCollection<LogEntry> PinnedLogEntries => _logDataSharingService.PinnedLogEntries;

    public List<string> Severities { get; } = ["Critical", "High", "Medium", "Low"];
    public List<double> Efforts { get; } = [0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5, 6, 6.5, 7, 7.5, 8, 8.5, 9, 9.5, 10];
    public DateTime CombinedDateTime => ReportDate.Date + ReportTime;


    public ReportViewModel(ILogDataSharingService logDataSharingService, IDialogService dialogService)
    {
        _logDataSharingService = logDataSharingService;
        _dialogService = dialogService;
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
    private void Submit()
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

            _dialogService.ShowMessage("Report submitted", "Successfully submitted your report.");
        }
        else
        {
            // Form invalid
        }
    }

    private void ValidateForm()
    {
        DeveloperNamePlaceholderColor = string.IsNullOrWhiteSpace(DeveloperName) ? Colors.Red : Colors.LightGray;
        SeverityTextColor = string.IsNullOrWhiteSpace(Severity) ? Colors.Red : Colors.Black;
        AnalysisPlaceholderColor = string.IsNullOrWhiteSpace(Analysis) ? Colors.Red : Colors.LightGray;
        RecomendationPlaceholderColor = string.IsNullOrWhiteSpace(Recommendation) ? Colors.Red : Colors.LightGray;

    }

    private bool IsFormValid()
    {
        return !string.IsNullOrWhiteSpace(DeveloperName)
               && !string.IsNullOrWhiteSpace(Severity)
               && !string.IsNullOrWhiteSpace(Analysis)
               && !string.IsNullOrWhiteSpace(Recommendation);
    }
}