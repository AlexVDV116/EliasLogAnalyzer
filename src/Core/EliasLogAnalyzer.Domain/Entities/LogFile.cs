using CommunityToolkit.Mvvm.ComponentModel;

namespace EliasLogAnalyzer.Domain.Entities;

public partial class LogFile : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;
    public string FileName { get; set; }
    public string FullPath { get; set; }
    public long FileSize { get; set; }
    public string Computer { get; set; }
    
    // One-to-many relationship with LogEntry
    private ICollection<LogEntry> _logEntries;
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
    
    public LogFile()
    {
        FileName = string.Empty;
        FullPath = string.Empty;
        FileSize = 0;
        Computer = string.Empty;
        _logEntries = new List<LogEntry>();
    }
}