using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using System.Collections.ObjectModel;

namespace EliasLogAnalyzer.MAUI.Services;

public class LogEntryAnalysisService(ILogDataSharingService logDataSharingService) : ILogEntryAnalysisService
{

    // Properties directly bound to the data sharing service, LogDataSharingService acts as the single source of truth for collections
    private ObservableCollection<LogEntry> LogEntries => logDataSharingService.LogEntries;
    private LogEntry? MarkedLogEntry => logDataSharingService.MarkedLogEntry;

    /// <summary>
    /// Calculates the Time Delta for a collection of log entries based on the difference with a marked entry.
    /// </summary>
    public void CalcDiffTicks()
    {

        foreach (var entry in LogEntries)
        {
            if (entry != MarkedLogEntry && MarkedLogEntry != null)
            {
                var timeDiff = (entry.LogTimeStamp.DateTime - MarkedLogEntry.LogTimeStamp.DateTime).TotalMilliseconds;
                var ticksDiff = entry.LogTimeStamp.Ticks - MarkedLogEntry.LogTimeStamp.Ticks;

                // Use ticks difference when time difference is zero
                entry.TimeDelta = timeDiff == 0 ? (int)ticksDiff : (int)timeDiff;
            }
            else
            {
                entry.TimeDelta = 0;
            }
        }
    }

    public void AnalyzeLogEntries()
    {
        if (MarkedLogEntry == null)
        {
            foreach (var entry in LogEntries) entry.Probability = null;
        }
        else
        {
            foreach (var entry in LogEntries)
            {
                if (entry != MarkedLogEntry)
                {
                    var probability = CalculateProbability(MarkedLogEntry, entry);
                    entry.Probability = (int)Math.Clamp(probability, 0, 100);
                }
            }
        }
    }

    /// <summary>
    /// Calculates the overall probability that two LogEntries are related by evaluating three aspects:
    /// time proximity, tick count similarity, and stack trace similarity. The method sums the probabilities derived from
    /// the differences in time (up to 5 seconds), exactness in tick alignment, and the percentage of matching stack trace lines.
    /// </summary>
    /// <param name="entry1">The first log entry to compare.</param>
    /// <param name="entry2">The second log entry to compare against the first.</param>
    /// <returns>The calculated probability, as a percentage, representing the likelihood that the two log entries are related based on their timestamps, tick counts, and stack traces.</returns>

    private static double CalculateProbability(LogEntry entry1, LogEntry entry2)
    {
        double probability = 0;
        probability += CalculateTimeProbability(entry1, entry2);
        probability += CalculateTicksProbability(entry1, entry2);
        probability += CalculateStackTraceProbability(entry1, entry2);
        return probability;
    }

    private static double CalculateTimeProbability(LogEntry entry1, LogEntry entry2)
    {
        var timeDiffSeconds = Math.Abs((entry1.LogTimeStamp.DateTime - entry2.LogTimeStamp.DateTime).TotalSeconds);
        if (timeDiffSeconds <= 5)
        {
            return Math.Max(25 - (timeDiffSeconds * 5), 0);  // Ensure non-negative
        }
        return 0;
    }

    private static double CalculateTicksProbability(LogEntry entry1, LogEntry entry2)
    {
        var ticksDiff = Math.Abs(entry1.LogTimeStamp.Ticks - entry2.LogTimeStamp.Ticks);
        if (ticksDiff == 0)
        {
            return 50;
        }
        // Prevent negative values and ensure result does not exceed the 0-50 range
        return Math.Max(25 - (ticksDiff / 10.0), 0);
    }

    private static double CalculateStackTraceProbability(LogEntry entry1, LogEntry entry2)
    {
        char[] separator = ['\r', '\n'];
        var stackTrace1 = entry1.Data.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        var stackTrace2 = entry2.Data.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        var matchingLines = stackTrace1.Count(line => stackTrace2.Contains(line));
        var totalLines = Math.Max(stackTrace1.Length, stackTrace2.Length);
        var similarityPercentage = (double)matchingLines / totalLines;

        // Convert similarity ratio to percentage and ensure non-negative
        return Math.Max(similarityPercentage * 100, 0);  // Ensure non-negative and within range
    }
}
