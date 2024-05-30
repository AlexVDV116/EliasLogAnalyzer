using EliasLogAnalyzer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliasLogAnalyzer.MAUI.Services.Contracts
{
    public interface IHtmlGeneratorService
    {
        string ConvertDataToHtml(LogEntry logEntry);
        string GenerateTimeLineHtml(IEnumerable<LogEntry> logEntries);
        string GeneratePieChartHtml(IEnumerable<LogEntry> logEntries);
    }

}
