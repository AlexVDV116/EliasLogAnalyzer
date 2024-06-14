namespace EliasLogAnalyzer.Domain.Entities;

public class BugReport
{
    public int BugReportId { get; init; }

    public DateTime ReportDateTime { get; init; } = DateTime.Now;
    public string Analysis { get; init; } = string.Empty;
    public string PossibleSolutions { get; init; } = string.Empty;
    public string Risk { get; init; } = string.Empty;
    public string Recommendation { get; init; } = string.Empty;
    public List<LogEntry> LogEntriesToInclude { get; set; } = [];
    public List<BugReportLogEntry> BugReportLogEntries { get; } = [];

}