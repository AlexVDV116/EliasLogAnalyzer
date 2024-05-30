using EliasLogAnalyzer.Domain.Entities;

namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface ILogFileParserService
{
    Task<(List<LogEntry> Entries, long FileSize)> ParseLogAsync(FileResult fileResult);
}