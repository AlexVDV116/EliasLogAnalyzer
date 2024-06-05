using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class LogEntriesViewModel(
    ILogger<LogEntriesViewModel> logger,
    ILogFileLoaderService logFileLoaderService,
    ILogFileParserService logFileParserService,
    ILogDataSharingService logDataSharingService,
    ILogEntryAnalysisService logEntryAnalysisService,
    IHtmlGeneratorService htmlGeneratorService)
    : ObservableObject
{

    #region Properties
    
    [ObservableProperty] private string _firstLogEntryDataHtml = string.Empty;
    [ObservableProperty] private string _secondLogEntryDataHtml = string.Empty;
    [ObservableProperty] private string _thirdLogEntryDataHtml = string.Empty;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private string _searchResultText = "0 / 0 👁️";
    [ObservableProperty] private string _emptyViewText = "No log entries found, please load logfiles.";
    [ObservableProperty] private string _debugLogTypeText = "Debug";
    [ObservableProperty] private string _informationLogTypeText = "Information";
    [ObservableProperty] private string _warningLogTypeText = "Warning";
    [ObservableProperty] private string _errorLogTypeText = "✓ Error";
    [ObservableProperty] private string _currentSortProperty = "DateTime";
    [ObservableProperty] private string _sortHeader = "";
    [ObservableProperty] private string _sortDateTimeHeaderText = "DateTime";
    [ObservableProperty] private string _sortLogTypeHeaderText = "LogType";
    [ObservableProperty] private string _sortThreadHeaderText = "Thread/No";
    [ObservableProperty] private string _sortSourceLocationHeaderText = "Source Location";
    [ObservableProperty] private string _sortSourceHeaderText = "Source";
    [ObservableProperty] private string _sortCategoryHeaderText = "Category";
    [ObservableProperty] private string _sortEventIdHeaderText = "Event ID";
    [ObservableProperty] private string _sortUserHeaderText = "User";
    [ObservableProperty] private string _sortComputerHeaderText = "Computer";

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _ascending = true;
    [ObservableProperty] private bool _isFirstLogEntrySelected;
    [ObservableProperty] private bool _isSecondLogEntrySelected;
    [ObservableProperty] private bool _isThirdLogEntrySelected;

    // Properties directly bound to the LogDataSharingService which acts as the single source of truth for collections
    private ObservableCollection<LogType> SelectedLogTypes => logDataSharingService.SelectedLogTypes;
    public ObservableCollection<object> SelectedLogEntries => logDataSharingService.SelectedLogEntries;
    public ObservableCollection<LogEntry> FilteredLogEntries => logDataSharingService.FilteredLogEntries;
    private LogEntry? MarkedLogEntry => logDataSharingService.MarkedLogEntry;

    #endregion

    #region Selection Management

    [RelayCommand]
    private void SelectionChanged()
    {
        // Ensure the marked log entry is always the first in the collection if it exists
        if (MarkedLogEntry != null && (SelectedLogEntries.Count == 0 || !SelectedLogEntries[0].Equals(MarkedLogEntry)))
        {
            SelectedLogEntries.Remove(MarkedLogEntry); // Remove if exists elsewhere
            SelectedLogEntries.Insert(0, MarkedLogEntry); // Insert at the top
        }

        // Manage selections to ensure only three entries are selected
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
                SelectedLogEntries.RemoveAt(1);
            }
            else
            {
                // If the first entry is not marked, remove it to make room for others
                SelectedLogEntries.RemoveAt(0);
            }
        }
    }
    
    #endregion
    
    #region Filtering

    partial void OnSearchTextChanged(string? oldValue, string newValue)
    {
        RefreshFilter();
    }
    
    // Filter the log entries based on the search text
    [RelayCommand]
    private void RefreshFilter()
    {
        logDataSharingService.UpdateFilter(string.IsNullOrWhiteSpace(SearchText) ? "" : SearchText);

        // Dynamic text to display the number of filtered entries and EmptyViewText based on search results
        SearchResultText = $"{FilteredLogEntries.Count} / {logDataSharingService.LogEntries.Count} 👁️";
        EmptyViewText = SearchText.Length > 0 && FilteredLogEntries.Count == 0
            ? "No results found. Try refining your search terms."
            : "No log entries found, please load logfiles.";
    }
    
    [RelayCommand]
    private void ChangeSelectedLogType(LogType logType)
    {
        if (!SelectedLogTypes.Remove(logType))
            SelectedLogTypes.Add(logType);

        UpdateLogTypeTexts();
        RefreshFilter();
    }
    
    #endregion

    #region Sorting

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
        
        logDataSharingService.SortByProperty(propertyName, Ascending);
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
    
        private void UpdateLogTypeTexts()
    {
        DebugLogTypeText = SelectedLogTypes.Contains(LogType.Debug) ? "✓ Debug" : "Debug";
        InformationLogTypeText = SelectedLogTypes.Contains(LogType.Information) ? "✓ Information" : "Information";
        WarningLogTypeText = SelectedLogTypes.Contains(LogType.Warning) ? "✓ Warning" : "Warning";
        ErrorLogTypeText = SelectedLogTypes.Contains(LogType.Error) ? "✓ Error" : "Error";
    }

    private void UpdateSelectedEntryData()
    {
        if (SelectedLogEntries.Count == 0)
        {
            FirstLogEntryDataHtml = string.Empty;
            SecondLogEntryDataHtml = string.Empty;
            ThirdLogEntryDataHtml = string.Empty;
            IsFirstLogEntrySelected = false;
            IsSecondLogEntrySelected = false;
            IsThirdLogEntrySelected = false;
        }
        else
        {
            (FirstLogEntryDataHtml, IsFirstLogEntrySelected) = SetLogEntryData(0);
            (SecondLogEntryDataHtml, IsSecondLogEntrySelected) = SetLogEntryData(1);
            (ThirdLogEntryDataHtml, IsThirdLogEntrySelected) = SetLogEntryData(2);
        }
    }
    
    private (string HtmlContent, bool IsSelected) SetLogEntryData(int index)
    {
        // Check if the index is within the bounds of the selected entries
        if (SelectedLogEntries.Count <= index) return (string.Empty, false);
        if (SelectedLogEntries[index] is not LogEntry logEntry) return (string.Empty, false);
            
        // Check if the current entry is the marked entry and is not in the first position
        if (logEntry == MarkedLogEntry && index != 0)
        {
            return (string.Empty, false); // Skip rendering the marked entry if not in the first position
        }

        var htmlContent = htmlGeneratorService.ConvertDataToHtml(logEntry);
        return (htmlContent, true);
    }

    
    #endregion
    
    #region Pinning and Marking

    [RelayCommand]
    private void PinLogEntry(LogEntry logEntry)
    {
        logDataSharingService.PinLogEntry(logEntry);
        logDataSharingService.SortByProperty(CurrentSortProperty, Ascending);
    }

    [RelayCommand]
    private void MarkLogEntry(LogEntry logEntry)
    {
        logDataSharingService.MarkLogEntry(logEntry);

        // If a log entry is marked, calculate the time difference between the marked entry and other entries
        if (MarkedLogEntry != null)
        {
            logEntryAnalysisService.CalcDiffTicks();
        }

        UpdateSelectedEntryData();
        logDataSharingService.SortByProperty(CurrentSortProperty, Ascending);
    }

    #endregion
    
    #region Loading and Parsing

    /// <summary>
    /// Initiates the process of loading and parsing log files asynchronously.
    /// </summary>
    [RelayCommand]
    private async Task LoadLogFilesAsync()
    {
        IsLoading = true;
        try
        {
            await LoadAndParseAllFilesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading or parsing files.");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    /// <summary>
    /// Loads log files and processes them concurrently adding them to the shared log data service.
    /// Uses Parallel.ForEachAsync to handle parsing in a parallel manner for better performance
    /// </summary>
    private async Task LoadAndParseAllFilesAsync()
    {
        var fileResults = await logFileLoaderService.LoadLogFilesAsync();
        await Parallel.ForEachAsync(fileResults, async (fileResult, _) =>
        {
            var logFile = await logFileParserService.ParseLogFileAsync(fileResult);
            logDataSharingService.AddLogFile(logFile);
            foreach (var logEntry in logFile.LogEntries)
            {
                logDataSharingService.AddLogEntry(logEntry);
            }
        });

        RefreshViewState();
    }

    /// <summary>
    /// Refreshes the view state by sorting and filtering entries.
    /// </summary>
    private void RefreshViewState()
    {
        logDataSharingService.SortByProperty("DateTime", Ascending);
        RefreshFilter();
    }

    #endregion
}