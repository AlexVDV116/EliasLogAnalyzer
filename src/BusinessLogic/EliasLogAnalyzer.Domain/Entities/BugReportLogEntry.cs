namespace EliasLogAnalyzer.Domain.Entities;

public class BugReportLogEntry
{
    public int Id { get; init; }

    public int BugReportId { get; init; }
    public BugReport BugReport { get; init; } = null!;

    public int LogEntryId { get; init; }
    public LogEntry LogEntry { get; init; } = null!;
}
