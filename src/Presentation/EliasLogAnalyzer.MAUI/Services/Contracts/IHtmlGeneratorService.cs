using EliasLogAnalyzer.BusinessLogic.Entities;

namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface IHtmlGeneratorService
{
    string ConvertDataToHtml(LogEntry logEntry);
    string GenerateTimeLineHtml();
    string GeneratePieChartHtml();
}


