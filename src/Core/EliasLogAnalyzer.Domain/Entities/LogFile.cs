using CommunityToolkit.Mvvm.ComponentModel;

namespace EliasLogAnalyzer.Domain.Entities;

public partial class LogFile : ObservableObject
{
    [ObservableProperty]
    private bool isSelected;
    public string FileName { get; set; }
    public IList<LogEntry> LogEntries { get; set; }
}