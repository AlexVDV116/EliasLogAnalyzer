using CommunityToolkit.Mvvm.ComponentModel;

namespace EliasLogAnalyzer.Domain.Entities;

public partial class LogFile : ObservableObject
{
    [ObservableProperty]
    private bool isSelected;
    public string FileName { get; set; }
    public string FullPath { get; set; }
    public long FileSize { get; set; }
    public string Computer { get; set; }
    
    private ICollection<LogEntry> _logEntries;
    public ICollection<LogEntry> LogEntries 
    { 
        get => _logEntries; 
        set
        {
            _logEntries = value;
            foreach (var entry in _logEntries)
            {
                entry.LogFile = this; // Set the parent reference
            }
        }
    }
}