using CommunityToolkit.Mvvm.ComponentModel;

namespace EliasLogAnalyzer.BusinessLogic.Entities;

// LogFile is a file with multiple LogEntries
public partial class LogEntry : ObservableObject
{
    public int LogEntryId { get; set; }
    public string Hash { get; set; } = string.Empty;
    public LogTimestamp LogTimeStamp { get; set; } = new LogTimestamp();
    public LogType LogType { get; set; } = LogType.None;
    public string ThreadNameOrNumber { get; set; } = string.Empty;
    // SourceLocation only gets set in debug mode
    public string SourceLocation { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int EventId { get; set; } = int.MaxValue;
    public string User { get; set; } = string.Empty;
    public string Computer { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;

    [ObservableProperty] private bool _isPinned = false;
    [ObservableProperty] private bool _isMarked = false;
    [ObservableProperty] private int? _timeDelta;
    [ObservableProperty] private int? _probability;

    // Many-to-one relationships
    public List<BugReport> BugReports { get; set; } = [];
    public List<BugReportLogEntry> BugReportLogEntries { get; } = [];
    public LogFile LogFile { get; set; } = new LogFile();

}
// Todo: Refactor code to make LogEntry + Logfile class private/internal, and expose only the necessary properties and methods