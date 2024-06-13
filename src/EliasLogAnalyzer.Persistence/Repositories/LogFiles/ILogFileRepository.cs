using EliasLogAnalyzer.Domain.Entities;

namespace EliasLogAnalyzer.Persistence.Repositories.LogFiles;

public interface ILogFileRepository
{
    Task<IEnumerable<LogFile>> GetAllLogFiles();
    Task<LogFile?> GetLogFileById(int id);
    Task<LogFile> AddLogFile(LogFile logFile);
    Task<bool> DeleteLogFile(int id);
}
