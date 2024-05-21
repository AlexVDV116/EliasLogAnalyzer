using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class LogEntriesViewModel : ObservableObject
{
    #region Fields

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

    public LogEntriesViewModel(ILogDataSharingService logDataSharingService,
        ILogEntryAnalysisService logEntryAnalysisService)
    {
        _logDataSharingService = logDataSharingService;
        _logEntryAnalysisService = logEntryAnalysisService;
        AggregateLogEntries();
        SortByProperty("DateTime");
        RefreshFilter();
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void SelectionChanged()
    {
        // Add the main marked logEntry if it's not in the selected entries
        if (MarkedLogEntry != null && !SelectedLogEntries.Contains(MarkedLogEntry))
        {
            SelectedLogEntries.Insert(0, MarkedLogEntry);
        }

        // Ensure only up to three log entries are selected, with priority given to the main marked log entry
        if (SelectedLogEntries.Count > 3)
        {
            var selectedEntriesCopy = SelectedLogEntries.Except(new[] { MarkedLogEntry }).ToList();
            while (selectedEntriesCopy.Count >= 3)
            {
                selectedEntriesCopy.RemoveAt(0); // Remove the oldest non-marked entry
            }

            SelectedLogEntries.Clear();
            if (MarkedLogEntry != null) SelectedLogEntries.Add(MarkedLogEntry);
            foreach (var entry in selectedEntriesCopy)
            {
                SelectedLogEntries.Add(entry);
            }
        }

        UpdateSelectedEntryData(); 
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
        if (propertyName == "DateTime")
        {
            LogEntries = Ascending
                ? new ObservableCollection<LogEntry>(LogEntries.OrderByDescending(x => x.IsPinned)
                    .ThenBy(x => x.LogTimeStamp.DateTime).ThenBy(x => x.LogTimeStamp.Ticks))
                : new ObservableCollection<LogEntry>(LogEntries.OrderByDescending(x => x.IsPinned)
                    .ThenByDescending(x => x.LogTimeStamp.DateTime).ThenByDescending(x => x.LogTimeStamp.Ticks));
        }
        else
        {
            var propertyInfo = typeof(LogEntry).GetProperty(propertyName);
            if (propertyInfo == null)
            {
                Console.WriteLine($"Property not found: {propertyName}");
                return;
            }

            LogEntries = Ascending
                ? new ObservableCollection<LogEntry>(LogEntries.OrderByDescending(x => x.IsPinned)
                    .ThenBy(x => propertyInfo.GetValue(x, null)))
                : new ObservableCollection<LogEntry>(LogEntries.OrderByDescending(x => x.IsPinned)
                    .ThenByDescending(x => propertyInfo.GetValue(x, null)));
        }

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
        // First handle the case where the same entry might be toggled
        if (MarkedLogEntry == logEntry)
        {
            // Toggle the mark state off if it's currently marked
            if (logEntry.IsMarked)
            {
                logEntry.IsMarked = false;
                SelectedLogEntries.Remove(logEntry);  // Remove it from selected entries
                MarkedLogEntry = null;  // Clear the marked entry
            }
        }
        else
        {
            // Unmark the previous marked entry if it exists
            if (MarkedLogEntry != null)
            {
                MarkedLogEntry.IsMarked = false;
                SelectedLogEntries.Remove(MarkedLogEntry);  // Remove the previously marked entry from selections
            }

            // Mark the new log entry
            logEntry.IsMarked = true;
            MarkedLogEntry = logEntry;  // Set as the new marked entry

            // Add the new main log entry to the selected entries if not already included
            if (!SelectedLogEntries.Contains(logEntry))
            {
                SelectedLogEntries.Add(logEntry);
            }

            // Ensure that no more than three log entries are selected, managing the collection accordingly
            ManageSelectedEntries();
        }

        OnPropertyChanged(nameof(SelectedLogEntries));
        UpdateSelectedEntryData();
    }



    #endregion

    #region Utility Methods

    private void ManageSelectedEntries()
    {
        while (SelectedLogEntries.Count > 3)
        {
            var oldestEntry = SelectedLogEntries
                .Where(e => e != MarkedLogEntry)  // Exclude the currently marked entry from removal
                .FirstOrDefault();

            if (oldestEntry != null)
            {
                SelectedLogEntries.Remove(oldestEntry);
            }
        }
    }

    private static string ConvertToHtml(LogEntry logEntry)
    {
        var dateTimeString = logEntry.LogTimeStamp.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
        var logTypeString = logEntry.LogType.ToString();
        var sourceString = logEntry.Source;
        var userString = logEntry.User;
        var computerString = logEntry.Computer;
        var descriptionString = logEntry.Description;
        var markedAsMainText = logEntry.IsMarked ? "✅" : string.Empty;


        return $@"
                <html>
                <head>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <style>
                        body {{ margin: 0; padding: 0; }}
                        pre {{ white-space: pre-wrap; word-wrap: break-word; }}
                    </style>
                </head>
                <body>
                    <h4><pre><b>{markedAsMainText} {System.Net.WebUtility.HtmlEncode(dateTimeString)}   -   {System.Net.WebUtility.HtmlEncode(logTypeString)}   -   {System.Net.WebUtility.HtmlEncode(sourceString)}   -   {System.Net.WebUtility.HtmlEncode(userString)}   -   {System.Net.WebUtility.HtmlEncode(computerString)}</b></pre></h4>
                    <pre><b>{System.Net.WebUtility.HtmlEncode(descriptionString)}</b></pre>
                    <pre>{System.Net.WebUtility.HtmlEncode(logEntry.Data)}</pre>
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

        foreach (var file in _logDataSharingService.SelectedLogFiles)
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
            SecondLogEntryDataHtml = ConvertToHtml(secondEntry);
            IsSecondLogEntrySelected = true;
        }
        else
        {
            IsSecondLogEntrySelected = false;
        }

        if (SelectedLogEntries.Count > 2 && SelectedLogEntries[2] is LogEntry thirdEntry)
        {
            ThirdLogEntryDataHtml = ConvertToHtml(thirdEntry);
            IsThirdLogEntrySelected = true;
        }
        else
        {
            IsThirdLogEntrySelected = false;
        }
    }

    #endregion
}