using CommunityToolkit.Mvvm.ComponentModel;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Microsoft.Maui;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class StatisticsViewModel : ObservableObject
{
    private readonly ILogDataSharingService _logDataSharingService;
    private readonly ILogEntryAnalysisService _logEntryAnalysisService;
    private readonly IHtmlGeneratorService _htmlGeneratorService;


    [ObservableProperty] private string _timelineHtml = string.Empty;
    [ObservableProperty] private string _pieChartHtml = string.Empty;
    [ObservableProperty] private bool _isLogEntryMarked = false;
    [ObservableProperty] private string _statisticsText = "Mark a LogEntry to analyse.";

    // Properties directly bound to the data sharing service, LogDataSharingService acts as the single source of truth for collections
    [ObservableProperty] public ObservableCollection<LogEntry> _logEntries = [];
    [ObservableProperty] public ObservableCollection<LogFile> _logFiles = [];
    [ObservableProperty] public ObservableCollection<LogType> _selectedLogTypes = [];
    [ObservableProperty] public ObservableCollection<object> _selectedLogEntries = [];
    [ObservableProperty] public ObservableCollection<LogFile> _selectedLogFiles = [];
    [ObservableProperty] public ObservableCollection<LogEntry> _filteredLogEntries = [];
    [ObservableProperty] public LogEntry? _markedLogEntry = null;


    public StatisticsViewModel(
        ILogDataSharingService logDataSharingService,
        ILogEntryAnalysisService logEntryAnalysisService,
        IHtmlGeneratorService htmlGeneratorService)
    {
        _logDataSharingService = logDataSharingService;
        _logEntryAnalysisService = logEntryAnalysisService;
        _htmlGeneratorService = htmlGeneratorService;


        LogEntries = _logDataSharingService.LogEntries;
        LogFiles = _logDataSharingService.LogFiles;
        SelectedLogTypes = _logDataSharingService.SelectedLogTypes;
        SelectedLogEntries = _logDataSharingService.SelectedLogEntries;
        SelectedLogFiles = _logDataSharingService.SelectedLogFiles;
        FilteredLogEntries = _logDataSharingService.FilteredLogEntries;
        MarkedLogEntry = _logDataSharingService.MarkedLogEntry;

        LogEntries.CollectionChanged += OnLogEntriesChanged;
        ((INotifyPropertyChanged)_logDataSharingService).PropertyChanged += OnMarkedLogEntryChanged;

        GenerateCharts();
        AnalyzeLogEntries();
    }

    private void OnMarkedLogEntryChanged(object sender, PropertyChangedEventArgs e)
    {
        MarkedLogEntry = _logDataSharingService.MarkedLogEntry;
        AnalyzeLogEntries();
        GenerateCharts();
    }

    private void OnLogEntriesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        LogEntries = _logDataSharingService.LogEntries;
        AnalyzeLogEntries();
        GenerateCharts();
    }

    private void AnalyzeLogEntries()
    {

        if (MarkedLogEntry != null)
        {
            StatisticsText = string.Empty;
            _logEntryAnalysisService.PerformAnalysis();
        }
    }

    private void GenerateCharts()
    {
        TimelineHtml = _htmlGeneratorService.GenerateTimeLineHtml(LogEntries);
        PieChartHtml = _htmlGeneratorService.GeneratePieChartHtml(LogEntries);
    }

}
