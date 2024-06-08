using EliasLogAnalyzer.BusinessLogic.Entities;

namespace EliasLogAnalyzer.API.Repositories.LogFiles;

public interface ILogFileRepository
{
    Task<IEnumerable<LogFile>> GetAllLogFiles();
    Task<LogFile?> GetLogFileById(int id);
    Task<LogFile> AddLogFile(LogFile logFile);
    Task<bool> DeleteLogFile(int id);
}
