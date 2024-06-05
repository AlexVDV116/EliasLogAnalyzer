using CommunityToolkit.Mvvm.ComponentModel;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;


namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class StatisticsViewModel : ObservableObject
{
    private readonly ILogDataSharingService _logDataSharingService;
    private readonly ILogEntryAnalysisService _logEntryAnalysisService;
    private readonly IHtmlGeneratorService _htmlGeneratorService;

    [ObservableProperty] private string _baseUrl = string.Empty;
    [ObservableProperty] private string _timelineHtml = string.Empty;
    [ObservableProperty] private string _pieChartHtml = string.Empty;
    [ObservableProperty] private bool _noLogEntryMarked = true;
    [ObservableProperty] private bool _logEntryMarked = true;
    [ObservableProperty] private string _statisticsText = "Mark a LogEntry to analyse relationship probabilities.";

    // Properties directly bound to the data sharing service, LogDataSharingService acts as the single source of truth for collections
    [ObservableProperty] private ObservableCollection<LogEntry> _logEntries = [];
    [ObservableProperty] private ObservableCollection<LogFile> _logFiles = [];
    [ObservableProperty] private ObservableCollection<LogType> _selectedLogTypes = [];
    [ObservableProperty] private ObservableCollection<object> _selectedLogEntries = [];
    [ObservableProperty] private ObservableCollection<LogEntry> _filteredLogEntries = [];
    [ObservableProperty] private LogEntry? _markedLogEntry;

    public StatisticsViewModel(
        ILogDataSharingService logDataSharingService,
        ILogEntryAnalysisService logEntryAnalysisService,
        IHtmlGeneratorService htmlGeneratorService)
    {
        _logDataSharingService = logDataSharingService;
        _logEntryAnalysisService = logEntryAnalysisService;
        _htmlGeneratorService = htmlGeneratorService;

        // Set BaseUrl based on platform to enable echart-min.js file to load in the WebView
        SetBaseUrl();

        LogEntries = _logDataSharingService.LogEntries;
        LogFiles = _logDataSharingService.LogFiles;
        SelectedLogTypes = _logDataSharingService.SelectedLogTypes;
        SelectedLogEntries = _logDataSharingService.SelectedLogEntries;
        FilteredLogEntries = _logDataSharingService.FilteredLogEntries;
        MarkedLogEntry = _logDataSharingService.MarkedLogEntry;
        NoLogEntryMarked = MarkedLogEntry == null;
        LogEntryMarked = MarkedLogEntry != null;

        LogEntries.CollectionChanged += OnLogEntriesChanged;
        ((INotifyPropertyChanged)_logDataSharingService).PropertyChanged += OnMarkedLogEntryChanged;

        GenerateCharts();
        AnalyzeLogEntries();
    }

    private void SetBaseUrl()
    {
#if MACCATALYST
        BaseUrl = Foundation.NSBundle.MainBundle.ResourcePath;
#else
        BaseUrl = string.Empty;
#endif
    }

    private void OnMarkedLogEntryChanged(object? sender, PropertyChangedEventArgs e)
    {
        MarkedLogEntry = _logDataSharingService.MarkedLogEntry;
        NoLogEntryMarked = MarkedLogEntry == null;
        LogEntryMarked = MarkedLogEntry != null;
        AnalyzeLogEntries();
        GenerateCharts();
    }

    private void OnLogEntriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
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
            _logEntryAnalysisService.AnalyzeLogEntries();
        }
    }

    private void GenerateCharts()
    {
        TimelineHtml = _htmlGeneratorService.GenerateTimeLineHtml(LogEntries);
        PieChartHtml = _htmlGeneratorService.GeneratePieChartHtml(LogEntries);
    }

}
