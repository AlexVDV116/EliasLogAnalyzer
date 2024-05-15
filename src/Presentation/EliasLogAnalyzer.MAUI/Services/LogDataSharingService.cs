using System.Collections.ObjectModel;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.Services;

/// <summary>
/// Manages and shares unique log files and entries across the application, ensuring data integrity and preventing duplicates.
/// Provides collections of log files and entries for view models to bind and interact with.
/// </summary>
public class LogDataSharingService : ILogDataSharingService
{
    private readonly HashSet<string> _loadedLogFileIdentifiers = [];
    private readonly HashSet<string> _loadedLogEntryIdentifiers = [];
    
    public ObservableCollection<LogFile> LogFiles { get; set; } = [];
    public ObservableCollection<LogEntry> LogEntries { get; set; } = [];
    public ObservableCollection<LogFile> SelectedLogFiles { get; set; } = [];
    
    public void AddLogFile(LogFile logFile)
    {
        if (_loadedLogFileIdentifiers.Add(logFile.FullPath))
        {
            LogFiles.Add(logFile);
            foreach (var logEntry in logFile.LogEntries)
            {
                AddLogEntry(logEntry);
            }
            Console.WriteLine($"Added {logFile.FullPath} to _loadedLogEntryIdentifiers");
        }
    }
    
    public void AddLogEntry(LogEntry logEntry)
    {
        var uniqueIdentifier = $"{logEntry.LogFile.FullPath}:{logEntry.LogTimeStamp.DateTime}";
        if (_loadedLogEntryIdentifiers.Add(uniqueIdentifier))
        {
            LogEntries.Add(logEntry);
        }
        Console.WriteLine($"Added {uniqueIdentifier} to _loadedLogEntryIdentifiers");
    }
    
    public void AddLogFileToSelected(LogFile logFile)
    {
        if (!SelectedLogFiles.Any(lf => lf.FullPath == logFile.FullPath))
        {
            SelectedLogFiles.Add(logFile);
        }
    }
    
}