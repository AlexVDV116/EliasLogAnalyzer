namespace EliasLogAnalyzer.Domain.Entities;

public class LogTimestamp
{
    public DateTime DateTime { get; set; } = DateTime.MinValue;

    public string DateTimeSortValue { get; set; } = string.Empty;

    // Ticks milliseconds since system has started
    public long Ticks { get; set; }
}