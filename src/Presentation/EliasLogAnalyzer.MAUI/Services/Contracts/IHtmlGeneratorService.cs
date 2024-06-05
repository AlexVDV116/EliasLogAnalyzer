using EliasLogAnalyzer.Domain.Entities;

namespace EliasLogAnalyzer.MAUI.Services.Contracts
{
    public interface IHtmlGeneratorService
    {
        string ConvertDataToHtml(LogEntry logEntry);
        string GenerateTimeLineHtml(IList<LogEntry> logEntries);
        string GeneratePieChartHtml(IList<LogEntry> logEntries);
    }

}
