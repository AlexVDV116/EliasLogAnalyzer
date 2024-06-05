using CommunityToolkit.Mvvm.ComponentModel;

namespace EliasLogAnalyzer.Domain.Entities;

public partial class LogFile : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;
    public string FileName { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public string Computer { get; set; } = string.Empty;

    // One-to-many relationship with LogEntry
    private ICollection<LogEntry> _logEntries = new List<LogEntry>();
    public ICollection<LogEntry> LogEntries 
    { 
        get => _logEntries; 
        set
        {
            _logEntries = value;
            foreach (var entry in _logEntries)
            {
                entry.LogFile = this; // Set the parent reference to this LogFile
            }
        }
    }
}