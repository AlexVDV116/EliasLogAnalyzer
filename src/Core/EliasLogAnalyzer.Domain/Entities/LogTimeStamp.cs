using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EliasLogAnalyzer.Domain.Entities;

public class LogTimestamp
{
    public DateTime DateTime { get; set; }
    public string DateTimeSortValue { get; set; }
    // Ticks milliseconds since system has started
    public long Ticks { get; set; }
}