using System.Collections.ObjectModel;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace EliasLogAnalyzer.MAUI.Services;

/// <summary>
/// Manages and shares unique log files and entries across the application, ensuring data integrity and preventing duplicates.
/// Provides collections of log files and entries for view models to bind and interact with.
/// </summary>
public class LogDataSharingService : ILogDataSharingService
{
    private readonly ILogger<LogFileParserService> _logger;
    private readonly HashSet<string> _loadedLogFileIdentifiers = [];
    private readonly HashSet<string> _loadedLogEntryIdentifiers = [];
    
    public ObservableCollection<LogFile> LogFiles { get; set; } = [];
    public ObservableCollection<LogEntry> LogEntries { get; set; } = [];
    public ObservableCollection<LogFile> SelectedLogFiles { get; set; } = [];
    
    public  LogDataSharingService(ILogger<LogFileParserService> logger)
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