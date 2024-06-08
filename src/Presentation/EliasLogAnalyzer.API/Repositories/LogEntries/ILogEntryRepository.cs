using EliasLogAnalyzer.BusinessLogic.Entities;

namespace EliasLogAnalyzer.API.Repositories.LogEntries;

public interface ILogEntryRepository
{
    Task<IEnumerable<LogEntry>> GetLogEntries();
    Task<LogEntry?> GetLogEntryById(int id);
    Task<LogEntry> AddOrUpdateLogEntry(LogEntry logEntry);
    Task<bool> DeleteLogEntry(int id);
}
