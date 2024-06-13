namespace EliasLogAnalyzer.Domain.Entities;

public class BugReport
{
    public int BugReportId { get; init; }
    public string DeveloperName { get; init; } = string.Empty;
    public string WorkstationName { get; init; } = string.Empty;
    public DateTime ReportDateTime { get; init; } = DateTime.Now;
    public string Situation { get; init; } = string.Empty;
    public string Observation { get; init; } = string.Empty;
    public string Expectation { get; init; } = string.Empty;
    public string Tag { get; init; } = string.Empty;
    public string Build { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public string Analysis { get; init; } = string.Empty;
    public string PossibleSolutions { get; init; } = string.Empty;
    public string WhatToTest { get; init; } = string.Empty;
    public double Effort { get; init; } = double.MaxValue;
    public string Risk { get; init; } = string.Empty;
    public string Workaround { get; init; } = string.Empty;
    public string Recommendation { get; init; } = string.Empty;
    public List<LogEntry> PinnedLogEntries { get; set; } = [];
    public List<BugReportLogEntry> BugReportLogEntries { get; } = [];

}