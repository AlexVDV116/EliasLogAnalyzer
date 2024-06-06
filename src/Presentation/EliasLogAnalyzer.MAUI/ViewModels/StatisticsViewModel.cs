using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    [ObservableProperty] public ObservableCollection<LogEntry> _sortedLogEntriesByProbability = [];
    [ObservableProperty] private string _baseUrl = string.Empty;
    [ObservableProperty] private string _timelineHtml = string.Empty;
    [ObservableProperty] private string _pieChartHtml = string.Empty;
    [ObservableProperty] private bool _noLogEntryMarked = true;
    [ObservableProperty] private bool _logEntryMarked = false;
    [ObservableProperty] private string _statisticsText = "Mark a LogEntry to analyse relationship probabilities.";

    // Properties directly bound to the LogDataSharingService which acts as the single source of truth for collections
    public ObservableCollection<LogEntry> LogEntries => _logDataSharingService.LogEntries;
    public ObservableCollection<object> SelectedLogEntries => _logDataSharingService.SelectedLogEntries;
    public ObservableCollection<LogEntry> FilteredLogEntries => _logDataSharingService.FilteredLogEntries;
    private LogEntry? MarkedLogEntry => _logDataSharingService.MarkedLogEntry;


    public StatisticsViewModel(
        ILogDataSharingService logDataSharingService,
        ILogEntryAnalysisService logEntryAnalysisService,
        IHtmlGeneratorService htmlGeneratorService)
    {
        _logDataSharingService = logDataSharingService;
        _logEntryAnalysisService = logEntryAnalysisService;
        _htmlGeneratorService = htmlGeneratorService;

        InitializeBindings();
        GenerateCharts();
    }

    [RelayCommand] private void PinLogEntry(LogEntry logEntry) => _logDataSharingService.PinLogEntry(logEntry);

    private void InitializeBindings()
    {
        SetBaseUrl();
        AttachCollectionChangeHandlers();
    }

    private void SetBaseUrl()
    {
#if MACCATALYST
        BaseUrl = Foundation.NSBundle.MainBundle.ResourcePath;
#else
        BaseUrl = string.Empty;
#endif
    }

    private void AttachCollectionChangeHandlers()
    {
        LogEntries.CollectionChanged += OnLogEntriesChanged;
        ((INotifyPropertyChanged)_logDataSharingService).PropertyChanged += OnMarkedLogEntryChanged;
    }

    public void InitializeSortedCollection()
    {
        if (LogEntries == null) return;

        var filteredAndSortedEntries = LogEntries
             .Where(entry => entry.Probability > 0)
             .OrderByDescending(entry => entry.Probability)
             .ToList();

        SortedLogEntriesByProbability = new ObservableCollection<LogEntry>(filteredAndSortedEntries);
    }

    private void OnMarkedLogEntryChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_logDataSharingService.MarkedLogEntry))
        {
            UpdatePageState();
            AnalyzeLogEntries();
            GenerateCharts();
            InitializeSortedCollection();
        }
    }

    private void UpdatePageState()
    {
        NoLogEntryMarked = MarkedLogEntry == null;
        LogEntryMarked = MarkedLogEntry != null;
    }

    private void OnLogEntriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        AnalyzeLogEntries();
        GenerateCharts();
    }

    private void AnalyzeLogEntries()
    {
        if (_logDataSharingService.MarkedLogEntry == null)
        {
            StatisticsText = "Mark a LogEntry to analyse relationship probabilities.";
            return;
        }

        StatisticsText = string.Empty;
        _logEntryAnalysisService.AnalyzeLogEntries();
        OnPropertyChanged(nameof(SortedLogEntriesByProbability));
    }

    private void GenerateCharts()
    {
        TimelineHtml = _htmlGeneratorService.GenerateTimeLineHtml();
        PieChartHtml = _htmlGeneratorService.GeneratePieChartHtml();
    }
}
