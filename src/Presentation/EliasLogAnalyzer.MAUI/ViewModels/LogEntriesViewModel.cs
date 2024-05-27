using System.Collections.ObjectModel;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Resources;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class LogEntriesViewModel : ObservableObject
{
    #region Fields

    private readonly ILogger<LogEntriesViewModel> _logger;
    private readonly ISettingsService _settingsService;
    private readonly ILogFileLoaderService _logFileLoaderService;
    private readonly ILogFileParserService _logFileParserService;
    private readonly ILogDataSharingService _logDataSharingService;
    private readonly ILogEntryAnalysisService _logEntryAnalysisService;

    #endregion

    #region Properties

    [ObservableProperty] private IList<object> _selectedLogEntries = [];
    [ObservableProperty] private ObservableCollection<LogEntry> _filteredLogEntries = [];
    [ObservableProperty] private ObservableCollection<LogType> _selectedLogTypes = [LogType.Error];
    [ObservableProperty] private LogEntry? _markedLogEntry;
    [ObservableProperty] private string _firstLogEntryDataHtml = string.Empty;
    [ObservableProperty] private string _secondLogEntryDataHtml = string.Empty;
    [ObservableProperty] private string _thirdLogEntryDataHtml = string.Empty;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private string _searchResultText = string.Empty;
    [ObservableProperty] private string _debugLogTypeText = "Debug";
    [ObservableProperty] private string _informationLogTypeText = "Information";
    [ObservableProperty] private string _warningLogTypeText = "Warning";
    [ObservableProperty] private string _errorLogTypeText = "✓ Error";
    [ObservableProperty] private string _currentSortProperty = "DateTime";
    [ObservableProperty] private string _sortHeader = "";
    [ObservableProperty] private string _currentSortDirection = "";
    [ObservableProperty] private string _sortDateTimeHeaderText = "DateTime";
    [ObservableProperty] private string _sortLogTypeHeaderText = "LogType";
    [ObservableProperty] private string _sortThreadHeaderText = "Thread/No";
    [ObservableProperty] private string _sortSourceLocationHeaderText = "Source Location";
    [ObservableProperty] private string _sortSourceHeaderText = "Source";
    [ObservableProperty] private string _sortCategoryHeaderText = "Category";
    [ObservableProperty] private string _sortEventIdHeaderText = "Event ID";
    [ObservableProperty] private string _sortUserHeaderText = "User";
    [ObservableProperty] private string _sortComputerHeaderText = "Computer";
    [ObservableProperty] private bool _ascending = true;
    [ObservableProperty] private bool _isSecondLogEntrySelected;
    [ObservableProperty] private bool _isThirdLogEntrySelected;

    public ObservableCollection<LogEntry> LogEntries { get; set; } = [];

    #endregion

    #region Constructor

    public LogEntriesViewModel(
        ILogger<LogEntriesViewModel> logger,
        ISettingsService settingsService,
        ILogFileLoaderService logFileLoaderService,
        ILogFileParserService logFileParserService,
        ILogDataSharingService logDataSharingService,
        ILogEntryAnalysisService logEntryAnalysisService
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settingsService = settingsService;
        _logFileLoaderService = logFileLoaderService;
        _logFileParserService = logFileParserService;
        _logDataSharingService = logDataSharingService;
        _logEntryAnalysisService = logEntryAnalysisService;
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void SelectionChanged()
    {
        // First, ensure the marked log entry is always the first in the collection if it exists
        if (MarkedLogEntry != null && !SelectedLogEntries.Contains(MarkedLogEntry))
        {
            SelectedLogEntries.Insert(0, MarkedLogEntry);
        }
        else if (MarkedLogEntry != null && SelectedLogEntries.IndexOf(MarkedLogEntry) != 0)
        {
            // Move the marked log entry to the first position if it's not already there
            SelectedLogEntries.Remove(MarkedLogEntry);
            SelectedLogEntries.Insert(0, MarkedLogEntry);
        }

        // Then, manage selections to ensure only three entries are selected
        ManageSelections();

        UpdateSelectedEntryData(); // Update UI to reflect changes
    }

    private void ManageSelections()
    {
        // Allow for 3 entries max, including the marked log entry if it exists
        while (SelectedLogEntries.Count > 3)
        {
            // If the first entry is the marked log entry, remove the second entry to make room
            if (MarkedLogEntry != null && SelectedLogEntries[0] == MarkedLogEntry)
            {
                // Remove the second entry, as the first one is marked
                SelectedLogEntries.RemoveAt(1);
            }
            else
            {
                // If the first entry is not marked, remove it to make room for others
                SelectedLogEntries.RemoveAt(0);
            }
        }
    }

    // Command to refresh filter whenever search text or selected log types change
    [RelayCommand]
    private void RefreshFilter()
    {
        var filtered = LogEntries.Where(entry =>
            SelectedLogTypes.Contains(entry.LogType) &&
            (string.IsNullOrWhiteSpace(SearchText) ||
             entry.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
             entry.User.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
             entry.Data.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        FilteredLogEntries = new ObservableCollection<LogEntry>(filtered);
        SearchResultText = $"{FilteredLogEntries.Count} / {LogEntries.Count} 🔎";
    }

    [RelayCommand]
    private void HeaderTapped(string propertyName)
    {
        if (CurrentSortProperty == propertyName)
        {
            // Toggle the sort direction if the same header is tapped again
            Ascending = !Ascending;
        }
        else
        {
            // Change the sort property and default direction to ascending
            CurrentSortProperty = propertyName;
            Ascending = true;
        }

        SortByProperty(propertyName);
        UpdateSortTexts(propertyName);
    }


    [RelayCommand]
    private void SetSortOrderAscending() => Ascending = true;

    [RelayCommand]
    private void SetSortOrderDescending() => Ascending = false;

    [RelayCommand]
    private void SortByProperty(string propertyName)
    {
        PropertyInfo? propertyInfo = null;
        if (propertyName != "DateTime")  // Check if the sorting is based on a property other than DateTime
        {
            propertyInfo = typeof(LogEntry).GetProperty(propertyName);
            if (propertyInfo == null)
            {
                Console.WriteLine($"Property not found: {propertyName}");
                return;
            }
        }

        LogEntries = new ObservableCollection<LogEntry>(
            LogEntries.OrderByDescending(x => x.IsPinned)   // Pinned entry first
                      .ThenByDescending(_ => !Ascending)            // Sort direction control
                      .ThenBy(x => Ascending ? (propertyName == "DateTime" ? x.LogTimeStamp.DateTime : propertyInfo?.GetValue(x, null)) : null)
                      .ThenByDescending(x => !Ascending ? (propertyName == "DateTime" ? x.LogTimeStamp.DateTime : propertyInfo?.GetValue(x, null)) : null)
        );

        RefreshFilter();
        OnPropertyChanged(nameof(LogEntries));
        UpdateSortOrder(propertyName);
    }

    [RelayCommand]
    private void ChangeSelectedLogType(LogType logType)
    {
        if (SelectedLogTypes.Contains(logType))
        {
            SelectedLogTypes.Remove(logType);
        }
        else
        {
            SelectedLogTypes.Add(logType);
        }

        UpdateLogTypeTexts();
        RefreshFilter();
    }

    [RelayCommand]
    private void PinLogEntry(LogEntry logEntry)
    {
        logEntry.IsPinned = !logEntry.IsPinned;
        SortByProperty(CurrentSortProperty);
    }

    [RelayCommand]
    private void MarkAsMainLogEntry(LogEntry logEntry)
    {
        if (MarkedLogEntry == logEntry && logEntry.IsMarked)
        {
            // Unmark the currently marked entry if entry is already marked
            logEntry.IsMarked = false;
            MarkedLogEntry = null;
            SelectedLogEntries.Clear();
            ResetDiffTicks();
        }
        else
        {
            // Unmark the previous marked entry if a new entry is marked
            if (MarkedLogEntry != null)
            {
                MarkedLogEntry.IsMarked = false;
                SelectedLogEntries.Remove(MarkedLogEntry);
                ResetDiffTicks();
            }

            logEntry.IsMarked = true;
            MarkedLogEntry = logEntry;
            SelectedLogEntries.Clear();
            SelectedLogEntries.Insert(0, logEntry);

            // Calculate DiffTicks based on the new marked entry
            _logEntryAnalysisService.CalcDiffTicks(logEntry, LogEntries);
        }

        UpdateSelectedEntryData();
        SortByProperty(CurrentSortProperty);
    }



    #endregion

    #region Utility Methods

    private void ResetDiffTicks()
    {
        foreach (var entry in LogEntries)
        {
            entry.TimeDelta = null;
        }
    }

    private string ConvertToHtml(LogEntry logEntry, bool compareWithMarked = false)
    {
        var dateTimeString = logEntry.LogTimeStamp.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
        var logTypeString = logEntry.LogType.ToString();
        var sourceString = logEntry.Source;
        var userString = logEntry.User;
        var computerString = logEntry.Computer;
        var descriptionString = logEntry.Description;
        var markedAsMainText = logEntry.IsMarked ? "\u2B50" : string.Empty;

        string dataString = compareWithMarked && MarkedLogEntry != null && MarkedLogEntry != logEntry
            ? _logEntryAnalysisService.GenerateDiff(MarkedLogEntry.Data, logEntry.Data)
            : System.Net.WebUtility.HtmlEncode(logEntry.Data);

        return $@"
            <html>
            <head>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <style>
                    body {{ margin: 0; padding: 0; font-family: 'Consolas', 'Courier New', monospace; background-color: #f9f9f9 ; color: #333; }}
                    pre {{ margin: 0; padding: 10px 20px; white-space: pre-wrap; word-wrap: break-word; }}
                    h4 {{ margin: 0; padding: 10px 20px; background-color: #e9e9e9; color: #333; border-bottom: 1px solid #ddd; }}
                </style>
            </head>
            <body>
                <h4><pre><b>{markedAsMainText} {System.Net.WebUtility.HtmlEncode(dateTimeString)}  -  {System.Net.WebUtility.HtmlEncode(logTypeString)}  -  {System.Net.WebUtility.HtmlEncode(sourceString)}  -  {System.Net.WebUtility.HtmlEncode(userString)}  -  {System.Net.WebUtility.HtmlEncode(computerString)}</b></pre></h4>
                <pre><b>Description: {System.Net.WebUtility.HtmlEncode(descriptionString)}</b></pre>
                <pre>Data: {dataString}</pre>
            </body>
            </html>
            ";
    }

    private void UpdateLogTypeTexts()
    {
        DebugLogTypeText = SelectedLogTypes.Contains(LogType.Debug) ? "✓ Debug" : "Debug";
        InformationLogTypeText = SelectedLogTypes.Contains(LogType.Information) ? "✓ Information" : "Information";
        WarningLogTypeText = SelectedLogTypes.Contains(LogType.Warning) ? "✓ Warning" : "Warning";
        ErrorLogTypeText = SelectedLogTypes.Contains(LogType.Error) ? "✓ Error" : "Error";
    }

    partial void OnSearchTextChanged(string value)
    {
        RefreshFilter();
    }

    private void UpdateSortOrder(string propertyName)
    {
        CurrentSortProperty = propertyName;
        CurrentSortDirection = Ascending ? "Ascending" : "Descending";
        UpdateSortTexts(propertyName);
    }


    // Updates the MenuBarItems text and CollectionView Header text to display current sort direction and property
    private void UpdateSortTexts(string sortProperty)
    {
        SortDateTimeHeaderText = "DateTime " + (sortProperty == "DateTime" ? (Ascending ? "▲" : "▼") : "");
        SortLogTypeHeaderText = "LogType " + (sortProperty == "LogType" ? (Ascending ? "▲" : "▼") : "");
        SortThreadHeaderText = "Thread/No " + (sortProperty == "ThreadNameOrNumber" ? (Ascending ? "▲" : "▼") : "");
        SortSourceLocationHeaderText =
            "Source Location " + (sortProperty == "SourceLocation" ? (Ascending ? "▲" : "▼") : "");
        SortSourceHeaderText = "Source " + (sortProperty == "Source" ? (Ascending ? "▲" : "▼") : "");
        SortCategoryHeaderText = "Category " + (sortProperty == "Category" ? (Ascending ? "▲" : "▼") : "");
        SortEventIdHeaderText = "Event ID " + (sortProperty == "EventId" ? (Ascending ? "▲" : "▼") : "");
        SortUserHeaderText = "User " + (sortProperty == "User" ? (Ascending ? "▲" : "▼") : "");
        SortComputerHeaderText = "Computer " + (sortProperty == "Computer" ? (Ascending ? "▲" : "▼") : "");
    }

    private void AggregateLogEntries()
    {
        LogEntries.Clear();
        var uniqueEntries = new HashSet<LogEntry>(LogEntries);

        foreach (var file in _logDataSharingService.LogFiles)
        {
            foreach (var entry in file.LogEntries)
            {
                if (uniqueEntries.Add(entry))
                {
                    LogEntries.Add(entry);
                }
            }
        }

        RefreshFilter();
    }

    private void UpdateSelectedEntryData()
    {
        // Check if there is a marked log entry to enable comparison
        var compareWithMarked = MarkedLogEntry != null;

        if (SelectedLogEntries.Count == 0)
        {
            FirstLogEntryDataHtml = string.Empty;
            SecondLogEntryDataHtml = string.Empty;
            ThirdLogEntryDataHtml = string.Empty;
            IsSecondLogEntrySelected = false;
            IsThirdLogEntrySelected = false;
            return;
        }

        if (SelectedLogEntries.Count > 0 && SelectedLogEntries[0] is LogEntry firstEntry)
        {
            FirstLogEntryDataHtml = ConvertToHtml(firstEntry);
        }

        if (SelectedLogEntries.Count > 1 && SelectedLogEntries[1] is LogEntry secondEntry)
        {
            SecondLogEntryDataHtml = ConvertToHtml(secondEntry, compareWithMarked);
            IsSecondLogEntrySelected = true;
        }
        else
        {
            IsSecondLogEntrySelected = false;
        }

        if (SelectedLogEntries.Count > 2 && SelectedLogEntries[2] is LogEntry thirdEntry)
        {
            ThirdLogEntryDataHtml = ConvertToHtml(thirdEntry, compareWithMarked);
            IsThirdLogEntrySelected = true;
        }
        else
        {
            IsThirdLogEntrySelected = false;
        }
    }

    /// <summary>
    /// Asynchronously loads multiple log files and parses them concurrently. 
    /// Each parsing operation is performed in its own task, ensuring that the parsing process is concurrent
    /// and efficient. 
    /// </summary>
    [RelayCommand]
    private async Task LoadLogFiles()
    {
        try
        {
            var fileResults = await _logFileLoaderService.LoadLogFilesAsync();

            // Create a list of tasks for parsing each new log file
            var parsingTasks = fileResults.Select(ParseLogFileAsync).ToList();

            // Await all tasks to complete
            var parsedLogFiles = await Task.WhenAll(parsingTasks);

            // Add the parsed log files to the shared service collection
            foreach (var logFile in parsedLogFiles)
            {
                _logDataSharingService.AddLogFile(logFile);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading files.");
        }
        AggregateLogEntries();
        SortByProperty("DateTime");
        RefreshFilter();
    }

    /// <summary>
    /// Asynchronously parses a single log file from a provided FileResult object. This method is designed
    /// to be called concurrently for multiple files, as each invocation operates independently, ensuring
    /// thread safety.
    /// </summary>
    /// <param name="fileResult">The file result from which the log file will be parsed.</param>
    /// <returns>A task that, when completed, returns a LogFile object containing all parsed entries.</returns>
    private async Task<LogFile> ParseLogFileAsync(FileResult fileResult)
    {
        var (logEntries, fileSize) = await _logFileParserService.ParseLogAsync(fileResult);
        var logFile = new LogFile
        {
            FileName = fileResult.FileName,
            FullPath = fileResult.FullPath,
            FileSize = fileSize,
            Computer = logEntries.FirstOrDefault()?.Computer ?? "Unknown",
            LogEntries = logEntries
        };

        foreach (var logEntry in logEntries)
        {
            logEntry.LogFile = logFile;
        }

        return logFile;
    }

    #endregion
}