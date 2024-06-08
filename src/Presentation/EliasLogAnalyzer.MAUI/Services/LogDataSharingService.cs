using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace EliasLogAnalyzer.MAUI.Services;

/// <summary>
/// Manages and shares unique log files and entries across the application, ensuring data integrity and preventing duplicates.
/// Provides collections of log files and entries for view models to bind and interact with. Acts as the single source of truth for
/// LogEntries and LogFiles collections.
/// </summary>
public partial class LogDataSharingService(ILogger<LogFileParserService> logger)
    : ObservableObject, ILogDataSharingService
{
    private readonly HashSet<string> _loadedLogFileIdentifiers = [];
    private readonly HashSet<string> _loadedLogEntryIdentifiers = [];

    [ObservableProperty] private ObservableCollection<LogFile> _logFiles = [];
    [ObservableProperty] private ObservableCollection<LogEntry> _logEntries = [];
    [ObservableProperty] private ObservableCollection<LogEntry> _pinnedLogEntries = [];
    [ObservableProperty] private ObservableCollection<object> _selectedLogEntries = [];
    [ObservableProperty] private ObservableCollection<LogEntry> _filteredLogEntries = [];
    [ObservableProperty] private ObservableCollection<LogType> _selectedLogTypes = [LogType.Error];
    [ObservableProperty] private LogEntry? _markedLogEntry;

    public void SortByProperty(string propertyName, bool ascending = true)
    {
        // Check for property existence unless it's a special case
        PropertyInfo? propertyInfo = null;
        if (propertyName != "DateTime" && propertyName != "Pin" && propertyName != "Marked")
        {
            propertyInfo = typeof(LogEntry).GetProperty(propertyName);
            if (propertyInfo == null)
            {
                logger.LogWarning("Property not found: {propertyName}", propertyName);
                return;
            }
        }

        // Configure the initial query, possibly with no sorting applied initially
        var entriesQuery = LogEntries.AsQueryable();

        if (propertyName == "Marked")
        {
            entriesQuery = entriesQuery
                .OrderByDescending(entry => entry.IsMarked)
                .ThenBy(entry => entry.IsPinned)
                .ThenBy(entry => entry.LogTimeStamp.DateTime);
        }
        else if (propertyName == "Pin")
        {
            entriesQuery = entriesQuery
                .OrderByDescending(entry => entry.IsPinned)
                .ThenBy(entry => entry.LogTimeStamp.DateTime);
        }
        else if (propertyName == "DateTime") // Handling DateTime separately because its property is nested inside LogTimeStamp
        {
            entriesQuery = ascending ?
                entriesQuery.OrderBy(entry => entry.LogTimeStamp.DateTime) :
                entriesQuery.OrderByDescending(entry => entry.LogTimeStamp.DateTime);
        }
        else if (propertyInfo != null)
        {
            entriesQuery = ascending ?
                entriesQuery.OrderBy(entry => propertyInfo.GetValue(entry, null)) :
                entriesQuery.OrderByDescending(entry => propertyInfo.GetValue(entry, null));
        }

        var sortedEntries = entriesQuery.ToList();
        LogEntries.Clear();
        foreach (var entry in sortedEntries)
        {
            LogEntries.Add(entry);
        }

        UpdateFilter();
    }


    public void UpdateFilter(string searchText = "")
    {
        List<LogEntry> newFilteredEntries;
        if (string.IsNullOrWhiteSpace(searchText))
        {
            newFilteredEntries = LogEntries
                .Where(entry => SelectedLogTypes.Contains(entry.LogType))
                .ToList();
        }
        else
        {
            newFilteredEntries = LogEntries
                .Where(entry => SelectedLogTypes.Contains(entry.LogType) &&
                                (entry.Source.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                 entry.User.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                 entry.Computer.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                 entry.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                 entry.Data.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        FilteredLogEntries.Clear();
        foreach (var entry in newFilteredEntries)
        {
            FilteredLogEntries.Add(entry);
        }
    }

    public void PinLogEntry(LogEntry entry)
    {
        entry.IsPinned = !entry.IsPinned;

        if (entry.IsPinned)
        {
            PinnedLogEntries.Add(entry);
        }
        else
        {
            PinnedLogEntries.Remove(entry);
        }
    }

    public void MarkLogEntry(LogEntry logEntry)
    {
        // Check if there's currently a marked entry and if it's different from the one being marked
        if (MarkedLogEntry != null && MarkedLogEntry != logEntry)
        {
            // Unmark the previous marked entry
            MarkedLogEntry.IsMarked = false;
        }

        // Toggle the mark state if it's the same entry being toggled
        if (logEntry == MarkedLogEntry)
        {
            logEntry.IsMarked = !logEntry.IsMarked;
            MarkedLogEntry = logEntry.IsMarked ? logEntry : null;
            ResetDiffTicks();
        }
        else
        {
            // Mark the new entry
            logEntry.IsMarked = true;
            MarkedLogEntry = logEntry;
        }

        // Clear selected entries to enforce the marked entry as the first
        SelectedLogEntries.Clear();
        if (MarkedLogEntry != null)
        {
            SelectedLogEntries.Add(MarkedLogEntry);
        }
    }

    private void ResetDiffTicks()
    {
        foreach (var entry in LogEntries)
        {
            entry.TimeDelta = null;
        }
    }

    public void AddLogFile(LogFile logFile)
    {
        if (!_loadedLogFileIdentifiers.Add(logFile.FullPath)) return;

        LogFiles.Add(logFile);
        foreach (var logEntry in logFile.LogEntries)
        {
            AddLogEntry(logEntry);
        }
    }

    /// <summary>
    /// Adds a log entry to the collection if it is unique.
    /// This method differentiates between log entries by creating a unique identifier that includes the file path,
    /// timestamp, and a hash of the log entry's data content. This ensures that entries with the same timestamp and
    /// header but different contents are recognized as distinct. Entries considered duplicates (same timestamp, headers,
    /// and data content) are not added, preventing data redundancy.
    /// </summary>
    /// <param name="logEntry">The log entry to add.</param>
    public void AddLogEntry(LogEntry logEntry)
    {
        // Include both the header information and a hash of the Data content to ensure uniqueness
        var uniqueIdentifier = logEntry.Hash;

        if (_loadedLogEntryIdentifiers.Add(uniqueIdentifier))
        {
            LogEntries.Add(logEntry);
        }
        else
        {
            logger.LogWarning("Duplicate log entry skipped: {UniqueIdentifier}", uniqueIdentifier);
        }
    }

    public void ClearAllLogs()
    {
        _loadedLogFileIdentifiers.Clear();
        _loadedLogEntryIdentifiers.Clear();

        LogFiles.Clear();
        LogEntries.Clear();
        SelectedLogEntries.Clear();
        FilteredLogEntries.Clear();
        MarkedLogEntry = null;
    }

}