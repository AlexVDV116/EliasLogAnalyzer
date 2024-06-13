using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace EliasLogAnalyzer.Persistence.Repositories.LogFiles;

public class LogFileRepository(EliasLogAnalyzerDbContext dbContext) : ILogFileRepository
{

    public async Task<IEnumerable<LogFile>> GetAllLogFiles()
    {
        return await dbContext.LogFiles.ToListAsync();
    }

    public async Task<LogFile?> GetLogFileById(int id)
    {
        return await dbContext.LogFiles.FindAsync(id);
    }

    public async Task<LogFile> AddLogFile(LogFile logFile)
    {
        dbContext.LogFiles.Add(logFile);
        await dbContext.SaveChangesAsync();
        return logFile;
    }

    public async Task<bool> DeleteLogFile(int id)
    {
        var logFile = await dbContext.LogFiles.FindAsync(id);
        if (logFile == null)
        {
            return false;
        }

        dbContext.LogFiles.Remove(logFile);
        int result = await dbContext.SaveChangesAsync();
        return result > 0;
    }
}