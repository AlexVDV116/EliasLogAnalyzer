using System.Text;

namespace EliasLogAnalyzer.Domain.Entities;

// LogFile is a file with multiple LogEntries
public class LogEntry
{
    public DateOnly Date { get; set; }
    // Time consists of hours, minutes, seconds and milliseconds ticks since system has started
    public string Time { get; set; }
    public string DateTimeSortValue { get; set; }
    public LogType LogType { get; set; }
    public string ThreadNameOrNumber { get; set; }
    // Only in debug mode
    public string SourceLocation { get; set; }
    public string Source { get; set; }
    public string Category { get; set; }
    public int EventId { get; set; }
    public string User { get; set; }
    public string Computer { get; set; }
    public string Description { get; set; }
    public string Data { get; set; }
}