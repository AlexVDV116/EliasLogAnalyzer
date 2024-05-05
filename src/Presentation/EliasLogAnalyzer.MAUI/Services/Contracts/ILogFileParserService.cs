using EliasLogAnalyzer.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;

namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface ILogFileParserService
{
    Task<List<LogEntry>> ParseLogAsync(FileResult fileResult);
}