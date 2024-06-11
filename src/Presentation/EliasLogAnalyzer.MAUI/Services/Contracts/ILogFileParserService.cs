using EliasLogAnalyzer.BusinessLogic.Entities;
using Microsoft.Maui.Storage;

namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface ILogFileParserService
{
    Task<LogFile> ParseLogFileAsync(FileResult fileResult);
}