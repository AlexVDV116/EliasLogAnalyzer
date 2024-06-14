using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.Domain.Entities;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using EliasLogAnalyzer.MAUI.Services.Contracts;


namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class StatisticsViewModel : ObservableObject
{
    private readonly ILogDataSharingService _logDataSharingService;
    private readonly IHtmlGeneratorService _htmlGeneratorService;

    [ObservableProperty] private ObservableCollection<LogEntry> _sortedLogEntriesByProbability = [];
    [ObservableProperty] private string _baseUrl = string.Empty;
    [ObservableProperty] private string _timelineHtml = string.Empty;
    [ObservableProperty] private string _pieChartHtml = string.Empty;
    [ObservableProperty] private string _barChartHtml = string.Empty;
    [ObservableProperty] private bool _noLogEntryMarked;
    [ObservableProperty] private bool _logEntryMarked;
    [ObservableProperty] private string _emptyCollectionViewText = "Mark a LogEntry to analyse relationship probabilities.";

    // Properties directly bound to the LogDataSharingService which acts as the single source of truth for collections
    private ObservableCollection<LogEntry> LogEntries => _logDataSharingService.LogEntries;
    private LogEntry? MarkedLogEntry => _logDataSharingService.MarkedLogEntry;

    public StatisticsViewModel(
        ILogDataSharingService logDataSharingService,
        IHtmlGeneratorService htmlGeneratorService)
    {
        _logDataSharingService = logDataSharingService;
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

    private void InitializeSortedCollection()
    {
        var filteredAndSortedEntries = LogEntries
             .Where(entry => entry.Probability > 0)
             .OrderByDescending(entry => entry.Probability)
             .ToList();

        SortedLogEntriesByProbability = new ObservableCollection<LogEntry>(filteredAndSortedEntries);
    }

    private void OnMarkedLogEntryChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(MarkedLogEntry)) return;
        UpdatePageState();
        GenerateCharts();
        InitializeSortedCollection();
    }

    private void UpdatePageState()
    {
        NoLogEntryMarked = MarkedLogEntry == null;
        LogEntryMarked = MarkedLogEntry != null;
    }

    private void OnLogEntriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        GenerateCharts();
    }

    private void GenerateCharts()
    {
        TimelineHtml = _htmlGeneratorService.GenerateTimeLineHtml();
        PieChartHtml = _htmlGeneratorService.GeneratePieChartHtml();
        BarChartHtml = _htmlGeneratorService.GenerateMultiBarChartHtml();
    }
}
