using System.Collections.ObjectModel;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.Services;

public class LogDataSharingService : ILogDataSharingService
{
    public ObservableCollection<LogFile> LogFiles { get; set; } = [];
    public ObservableCollection<LogEntry> LogEntries { get; set; } = [];

}