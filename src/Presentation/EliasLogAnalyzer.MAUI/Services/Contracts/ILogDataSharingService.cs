using System.Collections.ObjectModel;
using EliasLogAnalyzer.Domain.Entities;

namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface ILogDataSharingService
{
    ObservableCollection<LogFile> LogFiles { get; set; }
    ObservableCollection<LogEntry> LogEntries { get; set; }
    ObservableCollection<LogFile> SelectedLogFiles { get; set; }
}