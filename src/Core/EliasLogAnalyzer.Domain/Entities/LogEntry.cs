using CommunityToolkit.Mvvm.ComponentModel;

namespace EliasLogAnalyzer.Domain.Entities;

// LogFile is a file with multiple LogEntries
public partial class LogEntry : ObservableObject
{
    public LogTimestamp LogTimeStamp { get; set; }
    public LogType LogType { get; set; }
    public string ThreadNameOrNumber { get; set; }
    // SourceLocation only gets set in debug mode
    public string SourceLocation { get; set; }
    public string Source { get; set; }
    public string Category { get; set; }
    public int EventId { get; set; }
    public string User { get; set; }
    public string Computer { get; set; }
    public string Description { get; set; }
    public string Data { get; set; }

    [ObservableProperty] private bool _isPinned;
    [ObservableProperty] private bool _isMarked;
    [ObservableProperty] private int? _timeDelta;
    [ObservableProperty] private int? _probability;

    // Many-to-one relationship with LogFile
    public LogFile LogFile { get; set; }

    public LogEntry()
    {
        LogTimeStamp = new LogTimestamp();
        LogType = LogType.None;
        ThreadNameOrNumber = string.Empty;
        SourceLocation = string.Empty;
        Source = string.Empty;
        Category = string.Empty;
        EventId = 0;
        User = string.Empty;
        Computer = string.Empty;
        Description = string.Empty;
        Data = string.Empty;
        LogFile = new LogFile();
        IsPinned = false;
        IsMarked = false;
        TimeDelta = null;
        Probability = 0;
    }
}

// Todo: Refactor code to make LogEntry + Logfile class private/internal, and expose only the necessary properties and methods