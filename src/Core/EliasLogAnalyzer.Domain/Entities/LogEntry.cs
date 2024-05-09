using System.Text;

namespace EliasLogAnalyzer.Domain.Entities;

// LogFile is a file with multiple LogEntries
public class LogEntry
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

    public LogEntry()
    {
        LogTimeStamp = new LogTimestamp(); 
    }
}