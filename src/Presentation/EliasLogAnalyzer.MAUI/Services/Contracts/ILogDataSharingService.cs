using System.Collections.ObjectModel;
using EliasLogAnalyzer.Domain.Entities;

namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface ILogDataSharingService
{
    ObservableCollection<LogFile> LogFiles { get; set; }
    ObservableCollection<LogEntry> LogEntries { get; set; }
    ObservableCollection<LogEntry> PinnedLogEntries { get; set; }
    ObservableCollection<object> SelectedLogEntries { get; set; }
    ObservableCollection<LogEntry> FilteredLogEntries { get; set; }
    ObservableCollection<LogType> SelectedLogTypes { get; set; }
    LogEntry? MarkedLogEntry { get; set; }
    void AddLogFile(LogFile logFile);
    void AddLogEntry(LogEntry logEntry);
    void UpdateFilter(string searchText);
    void PinLogEntry(LogEntry entry);
    void MarkLogEntry(LogEntry entry);
    void SortByProperty(string propertyName, bool ascending = false);
    void ClearAllLogs();
    void RemoveLogEntry(LogEntry entry);
}