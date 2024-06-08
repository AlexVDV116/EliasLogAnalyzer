using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EliasLogAnalyzer.BusinessLogic.Entities;

public class BugReportLogEntry
{
    public int Id { get; set; }

    public int BugReportId { get; set; }
    public BugReport BugReport { get; set; } = null!;

    public int LogEntryId { get; set; }
    public LogEntry LogEntry { get; set; } = null!;
}
