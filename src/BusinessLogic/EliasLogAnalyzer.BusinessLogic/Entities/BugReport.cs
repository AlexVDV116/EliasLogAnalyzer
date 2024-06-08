namespace EliasLogAnalyzer.BusinessLogic.Entities;

public class BugReport
{
    public int BugReportId { get; set; }
    public string DeveloperName { get; set; } = string.Empty;
    public string WorkstationName { get; set; } = string.Empty;
    public DateTime ReportDateTime { get; set; } = DateTime.Now;
    public string Situation { get; set; } = string.Empty;
    public string Observation { get; set; } = string.Empty;
    public string Expectation { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string Build { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Analysis { get; set; } = string.Empty;
    public string PossibleSolutions { get; set; } = string.Empty;
    public string WhatToTest { get; set; } = string.Empty;
    public double Effort { get; set; } = double.MaxValue;
    public string Risk { get; set; } = string.Empty;
    public string Workaround { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public List<LogEntry> PinnedLogEntries { get; set; } = [];
    public List<BugReportLogEntry> BugReportLogEntries { get; } = [];

}