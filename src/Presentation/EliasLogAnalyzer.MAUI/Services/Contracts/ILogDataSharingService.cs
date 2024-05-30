using System.Collections.ObjectModel;
using System.ComponentModel;
using EliasLogAnalyzer.Domain.Entities;

namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface ILogDataSharingService
{
    ObservableCollection<LogFile> LogFiles { get; set; }
    ObservableCollection<LogEntry> LogEntries { get; set; }
    ObservableCollection<LogFile> SelectedLogFiles { get; set; }
    ObservableCollection<object> SelectedLogEntries { get; set; }
    ObservableCollection<LogEntry> FilteredLogEntries { get; set; }
    ObservableCollection<LogType> SelectedLogTypes { get; set; }
    LogEntry MarkedLogEntry { get; set; }
    void AddLogFile(LogFile logFile);
    void AddLogFileToSelected(LogFile logFile);
}
