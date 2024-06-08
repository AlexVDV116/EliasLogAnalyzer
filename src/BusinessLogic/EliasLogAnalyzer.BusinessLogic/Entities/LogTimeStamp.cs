namespace EliasLogAnalyzer.BusinessLogic.Entities;

public class LogTimestamp
{
    public DateTime DateTime { get; set; }

    public string DateTimeSortValue { get; set; } = string.Empty;

    // Ticks milliseconds since system has started
    public long Ticks { get; set; }

    public LogTimestamp()
    {
        DateTime = DateTime.MinValue;
        DateTimeSortValue = string.Empty;
        Ticks = 0;
    }
}