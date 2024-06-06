using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using EliasLogAnalyzer.Domain.Entities;
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
    [ObservableProperty] private ObservableCollection<object> _selectedLogEntries = [];
    [ObservableProperty] private ObservableCollection<LogEntry> _filteredLogEntries = [];
    [ObservableProperty] private ObservableCollection<LogType> _selectedLogTypes = [LogType.Error];
    [ObservableProperty] private LogEntry? _markedLogEntry;

    public void SortByProperty(string propertyName, bool ascending = false)
    {
        var propertyInfo = propertyName == "DateTime" ? null : typeof(LogEntry).GetProperty(propertyName);
        if (propertyName != "DateTime" && propertyInfo == null)
        {
            logger.LogWarning("Property not found: {propertyName}", propertyName);
            return;
        }

        var sorted = LogEntries
            .OrderByDescending(x => x.IsPinned) // Pinned entry first
            .ThenBy(x =>
                ascending
                    ? (propertyName == "DateTime" ? x.LogTimeStamp.DateTime : propertyInfo?.GetValue(x, null))
                    : null)
            .ThenByDescending(x =>
                !ascending
                    ? (propertyName == "DateTime" ? x.LogTimeStamp.DateTime : propertyInfo?.GetValue(x, null))
                    : null)
            .ToList();

        LogEntries.Clear();
        foreach (var entry in sorted)
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

    public void AddLogEntry(LogEntry logEntry)
    {
        var uniqueIdentifier = $"{logEntry.LogFile.FullPath}:{logEntry.LogTimeStamp.DateTime}";
        if (_loadedLogEntryIdentifiers.Add(uniqueIdentifier))
        {
            LogEntries.Add(logEntry);
        }
    }
}