using System.Collections.ObjectModel;
using System.Reflection;
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

        // Ensure only up to three log entries are selected, prioritizing the main marked log entry
        if (SelectedLogEntries.Count > 3)
        {
            // Create a copy of selected entries excluding the main marked log entry
            var selectedEntriesCopy = SelectedLogEntries.Where(e => e != MarkedLogEntry).ToList();

            // Remove excess entries if any, ensuring we do not exceed three entries
            while (SelectedLogEntries.Count + selectedEntriesCopy.Count > 3)
            {
                var itemToRemove = selectedEntriesCopy.FirstOrDefault();
                if (itemToRemove != null)
                {
                    SelectedLogEntries.Remove(itemToRemove);
                    selectedEntriesCopy.Remove(itemToRemove);
                }
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
            LogEntries.OrderByDescending(x => x == MarkedLogEntry)  // Marked entry first
                      .ThenByDescending(x => x.IsPinned)            // Then all pinned entries
                      .ThenByDescending(x => !Ascending)            // Sort direction control
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
            // Unmark the currently marked entry
            logEntry.IsMarked = false;
            MarkedLogEntry = null;
        }
        else
        {
            // Unmark the previous marked entry
            if (MarkedLogEntry != null)
            {
                MarkedLogEntry.IsMarked = false;
                SelectedLogEntries.Remove(MarkedLogEntry);
            }

            logEntry.IsMarked = true;
            MarkedLogEntry = logEntry; 

            // Add the new main log entry to the selected entries if not already included
            if (!SelectedLogEntries.Contains(logEntry))
            {
                SelectedLogEntries.Add(logEntry);
            }

            ManageSelectedEntries();
        }

        OnPropertyChanged(nameof(SelectedLogEntries));
        OnPropertyChanged(nameof(MarkedLogEntry));
        UpdateSelectedEntryData();
        SortByProperty(CurrentSortProperty);
    }



    #endregion

    #region Utility Methods

    // Ensure that no more than three log entries are selected, managing the collection accordingly
    private void ManageSelectedEntries()
    {
        while (SelectedLogEntries.Count > 3)
        {
            var oldestEntry = SelectedLogEntries
                .Where(e => e != MarkedLogEntry)
                .FirstOrDefault();

            if (oldestEntry != null)
            {
                SelectedLogEntries.Remove(oldestEntry);
            }
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
            var markedAsMainText = logEntry.IsMarked ? "✅" : string.Empty;

        string dataString = compareWithMarked && MarkedLogEntry != null && MarkedLogEntry != logEntry
            ? _logEntryAnalysisService.GenerateDiff(MarkedLogEntry.Data, logEntry.Data) 
            : System.Net.WebUtility.HtmlEncode(logEntry.Data);


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
                    <pre>{dataString}</pre>
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

    #endregion
}