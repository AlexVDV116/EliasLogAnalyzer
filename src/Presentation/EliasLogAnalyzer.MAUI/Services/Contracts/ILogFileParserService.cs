using EliasLogAnalyzer.BusinessLogic.Entities;

namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface ILogFileParserService
{
    Task<LogFile> ParseLogFileAsync(FileResult fileResult);
}