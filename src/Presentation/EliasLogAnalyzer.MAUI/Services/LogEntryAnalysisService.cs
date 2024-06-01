using CommunityToolkit.Mvvm.ComponentModel;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using System.Collections.ObjectModel;

namespace EliasLogAnalyzer.MAUI.Services;

public class LogEntryAnalysisService : ILogEntryAnalysisService
{
    private readonly ILogDataSharingService _logDataSharingService;

    // Properties directly bound to the data sharing service, LogDataSharingService acts as the single source of truth for collections
    private ObservableCollection<LogEntry> LogEntries = [];
    private LogEntry? MarkedLogEntry = null;

    public LogEntryAnalysisService(ILogDataSharingService logDataSharingService)
    {
        _logDataSharingService = logDataSharingService;

        LogEntries = _logDataSharingService.LogEntries;
        MarkedLogEntry = _logDataSharingService.MarkedLogEntry;

    }

    /// <summary>
    /// Calculates the Time Delta for a collection of log entries based on the difference with a marked entry.
    /// </summary>
    /// <param name="markedEntry">The log entry marked for comparison.</param>
    /// <param name="entries">The list of all entries to compare against the marked entry.</param>
    public void CalcDiffTicks()
    {
        MarkedLogEntry = _logDataSharingService.MarkedLogEntry;

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
        if (MarkedLogEntry == null) return;

        foreach (var entry in LogEntries)
        {
            if (entry != MarkedLogEntry)
            {
                double probability = CalculateProbability(MarkedLogEntry, entry);
                entry.Propability = (int)Math.Clamp(probability, 0, 100);
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
        var stackTrace1 = entry1.Data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var stackTrace2 = entry2.Data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        var matchingLines = stackTrace1.Count(line => stackTrace2.Contains(line));
        var totalLines = Math.Max(stackTrace1.Length, stackTrace2.Length);
        var similarityPercentage = (double)matchingLines / totalLines;

        // Convert similarity ratio to percentage and ensure non-negative
        return Math.Max(similarityPercentage * 100, 0);  // Ensure non-negative and within range
    }
}
