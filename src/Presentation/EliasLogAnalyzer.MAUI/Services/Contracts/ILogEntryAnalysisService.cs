using EliasLogAnalyzer.Domain.Entities;

namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface ILogEntryAnalysisService
{
    /// <summary>
    /// Calculates the relationship scores between a main LogEntry and a list of other LogEntries.
    /// Each score reflects the relationship between the main entry and one of the other entries.
    /// </summary>
    /// <param name="mainEntry">The main LogEntry to compare against.</param>
    /// <param name="otherEntries">A list of LogEntries to be compared with the main entry.</param>
    /// <returns>A list of relationship scores as percentages, matching the order of otherEntries.</returns>
    List<double> CalculateRelationshipScores(LogEntry mainEntry, List<LogEntry> otherEntries);

    string GenerateDiff(string oldText, string newText);

    void CalcDiffTicks(LogEntry markedEntry, IEnumerable<LogEntry> entries);
}