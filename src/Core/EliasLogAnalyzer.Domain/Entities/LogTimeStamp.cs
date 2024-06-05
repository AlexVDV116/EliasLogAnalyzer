using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.CompilerServices;

namespace EliasLogAnalyzer.Domain.Entities;

public class LogTimestamp
{
    public DateTime DateTime { get; set; }

    public string DateTimeSortValue { get; set; } = string.Empty;

    // Ticks milliseconds since system has started
    public long Ticks { get; set; }
}