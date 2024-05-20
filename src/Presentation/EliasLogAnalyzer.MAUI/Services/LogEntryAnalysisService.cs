using System.Text;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.Services;

public class LogEntryAnalysisService : ILogEntryAnalysisService
{
    public List<double> CalculateRelationshipScores(LogEntry mainEntry, List<LogEntry> otherEntries)
    {
        var scores = new List<double>(); 

        foreach (var entry in otherEntries) 
        {
            var similarityScore = CalculateSimilarity(mainEntry.Data, entry.Data);
            var timeScore = CalculateTimeProximity(mainEntry.LogTimeStamp, entry.LogTimeStamp); 
            var finalScore = (similarityScore * 0.7) + (timeScore * 0.3); 
            scores.Add(finalScore);
        }

        return scores;
    }

    /// <summary>
    /// Calculates the textual similarity between two strings using the DiffPlex library.
    /// This method computes the difference between the 'mainData' and 'compareData' strings,
    /// determining the proportion of text that remains unchanged between them. The similarity
    /// is quantified as a percentage of the count of unchanged characters to the total number of characters
    /// in the longer of the two strings, providing a measure of how similar the two strings are.
    /// </summary>
    /// <param name="mainData">The data string from the main LogEntry.</param>
    /// <param name="compareData">The data string from the comparing LogEntry.</param>
    /// <returns>A similarity score between 0 and 100, where 100 indicates exact similarity (100% matching)
    /// and 0 indicates no similarity (0% matching).</returns>
    private double CalculateSimilarity(string mainData, string compareData)
    {
        if (string.IsNullOrEmpty(mainData) || string.IsNullOrEmpty(compareData))
            return 0.0;

        var differ = new Differ();
        var builder = new InlineDiffBuilder(differ);
        var diffResult = builder.BuildDiffModel(mainData, compareData);

        int totalCharacters = Math.Max(mainData.Length, compareData.Length);
        int matchedCharacters = diffResult.Lines.Count(line => line.Type == ChangeType.Unchanged);

        // Calculate the similarity as a percentage and maintain precision with double
        return totalCharacters > 0 ? ((double)matchedCharacters / totalCharacters) * 100.0 : 0.0;
    }

    /// <summary>
    /// Calculates the time proximity between two LogTimestamps based on their DateTime and Ticks properties.
    /// This method normalizes the score to a 0-100% range based on whether the DateTime properties are within the same second and how close their Ticks are.
    /// </summary>
    /// <param name="mainTimestamp">The timestamp from the main LogEntry.</param>
    /// <param name="compareTimestamp">The timestamp from the comparing LogEntry.</param>
    /// <returns>A score from 0% to 100%, where 100% indicates exact same time and Ticks, and 0% indicates a difference of more than one second or completely different Ticks.</returns>
    private double CalculateTimeProximity(LogTimestamp mainTimestamp, LogTimestamp compareTimestamp)
    {
        // Check if the entries are within the same second
        if (mainTimestamp.DateTime.ToString("yyyy-MM-dd HH:mm:ss") !=
            compareTimestamp.DateTime.ToString("yyyy-MM-dd HH:mm:ss")) return 0;
        
        // Compare the Ticks for exact timing within the second
        var tickDifference = Math.Abs(mainTimestamp.Ticks - compareTimestamp.Ticks); 
        var tickScore = Math.Max(0, 1000 - tickDifference) / 10.0; // Normalize tick difference (0 to 100 scale)
        return tickScore; // Return normalized score as a percentage 

    }
    
    public string GenerateDiff(string before, string after)
    {
        var diffBuilder = new InlineDiffBuilder(new Differ());
        var diff = diffBuilder.BuildDiffModel(before, after);

        var sb = new StringBuilder();
        foreach (var line in diff.Lines)
        {
            switch (line.Type)
            {
                case ChangeType.Inserted:
                    sb.Append($"<span style='color: green;'>+ {line.Text}</span><br/>");
                    break;
                case ChangeType.Deleted:
                    sb.Append($"<span style='color: red;'>- {line.Text}</span><br/>");
                    break;
                case ChangeType.Unchanged:
                    sb.Append($"<span>{line.Text}</span><br/>");
                    break;
                case ChangeType.Modified:
                    sb.Append($"<span style='color: blue;'>~ {line.Text}</span><br/>");
                    break;
            }
        }
        return sb.ToString();
    }
}