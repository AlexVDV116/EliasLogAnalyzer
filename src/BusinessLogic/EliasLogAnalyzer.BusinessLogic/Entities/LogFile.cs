using CommunityToolkit.Mvvm.ComponentModel;

namespace EliasLogAnalyzer.BusinessLogic.Entities;

public partial class LogFile : ObservableObject
{
    public int LogFileId { get; set; }
    public string Hash { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public string Computer { get; set; } = string.Empty;

    [ObservableProperty] private bool _isSelected;

    public ICollection<LogEntry> LogEntries = [];
}