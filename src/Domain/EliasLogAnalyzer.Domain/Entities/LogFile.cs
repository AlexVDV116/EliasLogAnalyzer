using CommunityToolkit.Mvvm.ComponentModel;

namespace EliasLogAnalyzer.Domain.Entities;

public partial class LogFile : ObservableObject
{
    public int LogFileId { get; init; }
    public string Hash { get; set; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string FullPath { get; init; } = string.Empty;
    public string Computer { get; init; } = string.Empty;

    [ObservableProperty] private bool _isSelected;

    public ICollection<LogEntry> LogEntries = [];
}