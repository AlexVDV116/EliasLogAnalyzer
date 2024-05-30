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
public partial class LogDataSharingService : ObservableObject, ILogDataSharingService
{
    private readonly ILogger<LogFileParserService> _logger;
    private readonly HashSet<string> _loadedLogFileIdentifiers = [];
    private readonly HashSet<string> _loadedLogEntryIdentifiers = [];

    [ObservableProperty] private ObservableCollection<LogFile> _logFiles = [];
    [ObservableProperty] private ObservableCollection<LogEntry> _logEntries = [];
    [ObservableProperty] private ObservableCollection<LogFile> _selectedLogFiles = [];
    [ObservableProperty] private ObservableCollection<object> _selectedLogEntries = [];
    [ObservableProperty] private ObservableCollection<LogEntry> _filteredLogEntries = [];
    [ObservableProperty] private ObservableCollection<LogType> _selectedLogTypes = [LogType.Error];
    private LogEntry _markedLogEntry;

    public LogEntry MarkedLogEntry
    {
        get => _markedLogEntry;
        set
        {
            if (_markedLogEntry != value)
            {
                if (_markedLogEntry != null)
                {
                    _markedLogEntry.IsMarked = false; // Unmark the previous entry
                }
                _markedLogEntry = value;
                OnPropertyChanged();
                if (_markedLogEntry != null)
                {
                    _markedLogEntry.IsMarked = true; // Mark the new entry
                }
            }
        }
    }


    public LogDataSharingService(ILogger<LogFileParserService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void AddLogFile(LogFile logFile)
    {
        if (_loadedLogFileIdentifiers.Add(logFile.FullPath))
        {
            LogFiles.Add(logFile);
            foreach (var logEntry in logFile.LogEntries)
            {
                AddLogEntry(logEntry);
            }
            _logger.LogInformation("Added {logFile} to _loadedLogEntryIdentifiers.", logFile.FullPath);
        }
    }

    public void AddLogEntry(LogEntry logEntry)
    {
        var uniqueIdentifier = $"{logEntry.LogFile.FullPath}:{logEntry.LogTimeStamp.DateTime}";
        if (_loadedLogEntryIdentifiers.Add(uniqueIdentifier))
        {
            LogEntries.Add(logEntry);
        }
        _logger.LogInformation("Added {uniqueIdentifier} to _loadedLogEntryIdentifiers.", uniqueIdentifier);
    }

    public void AddLogFileToSelected(LogFile logFile)
    {
        if (SelectedLogFiles.All(lf => lf.FullPath != logFile.FullPath))
        {
            SelectedLogFiles.Add(logFile);
        }
    }

}