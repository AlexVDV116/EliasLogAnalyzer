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

    public Dictionary<string, List<LogEntry>> PerformAnalysis()
    {

        var tempResults = new Dictionary<string, List<LogEntry>>();
        foreach (var probabilityRange in new[] { "100%", "> 75%", "> 50%", "> 25%", "< 25%" })
        {
            tempResults[probabilityRange] = [];
        }

        foreach (var entry in LogEntries)
        {
            if (entry != MarkedLogEntry && MarkedLogEntry != null)
            {
                var probability = CalculateRelationProbability(MarkedLogEntry, entry);
                entry.Propability = (int)probability;  // Set the probability value
                var range = GetProbabilityRange(probability);
                tempResults[range].Add(entry);
            }
        }

        // Only add non-empty groups to results
        var results = new Dictionary<string, List<LogEntry>>();
        foreach (var range in tempResults)
        {
            if (range.Value.Count > 0)
            {
                results[range.Key] = range.Value;
            }
        }

        return results;
    }


    private static string GetProbabilityRange(double probability)
    {
        if (probability == 100) return "100%";
        if (probability > 75) return "> 75%";
        if (probability > 50) return "> 50%";
        if (probability > 25) return "> 25%";
        return "< 25%";
    }


    private static double CalculateRelationProbability(LogEntry entry1, LogEntry entry2)
    {
        double probability = 0;

        // Check if they are within 5 seconds of each other
        var timeDiff = Math.Abs((entry1.LogTimeStamp.DateTime - entry2.LogTimeStamp.DateTime).TotalSeconds);
        if (timeDiff <= 5)
        {
            probability += (5 - timeDiff) * 5; // Add 5% for each second within 5 seconds
        }

        // Check if they are logged at the same second
        if (entry1.LogTimeStamp.DateTime.Second == entry2.LogTimeStamp.DateTime.Second)
        {
            probability += 25;
        }

        // Compare ticks
        var ticksDiff = Math.Abs(entry1.LogTimeStamp.Ticks - entry2.LogTimeStamp.Ticks);
        if (ticksDiff == 0)
        {
            probability += 25;
        }
        else
        {
            probability += 25 - Math.Min(ticksDiff / 10.0, 25); // Subtract 1% for every 10 ticks difference up to 25%
        }

        // Compare stack traces
        var stackTrace1 = entry1.Data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var stackTrace2 = entry2.Data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        int matchingLines = stackTrace1.Count(line1 => stackTrace2.Any(line2 => line1.Contains(line2) || line2.Contains(line1)));
        int totalLines = Math.Max(stackTrace1.Length, stackTrace2.Length);

        var stackTraceSimilarity = (double)matchingLines / totalLines;
        if (stackTraceSimilarity >= 0.5)
        {
            probability += 50;
        }
        else
        {
            probability -= (0.5 - stackTraceSimilarity) * 100 * 5; // Subtract 5% for every line less than 50% similarity
        }

        return probability;
    }

}
