using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.Services;

public class LogEntryAnalysisService : ILogEntryAnalysisService
{
    /// <summary>
    /// Calculates the Time Delta for a collection of log entries based on the difference with a marked entry.
    /// </summary>
    /// <param name="markedEntry">The log entry marked for comparison.</param>
    /// <param name="entries">The list of all entries to compare against the marked entry.</param>
    public void CalcDiffTicks(LogEntry markedEntry, IEnumerable<LogEntry> entries)
    {
        foreach (var entry in entries)
        {
            if (entry != markedEntry)
            {
                var timeDiff = (entry.LogTimeStamp.DateTime - markedEntry.LogTimeStamp.DateTime).TotalMilliseconds;
                var ticksDiff = entry.LogTimeStamp.Ticks - markedEntry.LogTimeStamp.Ticks;

                // Use ticks difference when time difference is zero
                entry.TimeDelta = timeDiff == 0 ? (int)ticksDiff : (int)timeDiff;
            }
            else
            {
                entry.TimeDelta = 0;
            }
        }
    }
}
