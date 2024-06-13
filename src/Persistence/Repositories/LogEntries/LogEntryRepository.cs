using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace EliasLogAnalyzer.Persistence.Repositories.LogEntries
{
    public class LogEntryRepository(EliasLogAnalyzerDbContext dbContext) : ILogEntryRepository
    {

        public async Task<IEnumerable<LogEntry>> GetLogEntries()
        {
            var logEntries = await dbContext.LogEntries.ToListAsync();
            return logEntries;
        }

        async Task<LogEntry?> ILogEntryRepository.GetLogEntryById(int id)
        {
            return await dbContext.LogEntries.FindAsync(id);
        }

        public async Task<LogEntry> AddOrUpdateLogEntry(LogEntry logEntry)
        {
            // Check if the LogFile exists and use the existing instance
            var existingLogFile = await dbContext.LogFiles.FirstOrDefaultAsync(lf => lf.Hash == logEntry.LogFile.Hash);
            if (existingLogFile == null)
            {
                dbContext.LogFiles.Add(logEntry.LogFile);
            }
            else
            {
                logEntry.LogFile = existingLogFile; // Use the existing LogFile
            }

            // Now add or update the LogEntry
            var existingLogEntry = await dbContext.LogEntries
                .Include(le => le.LogFile)
                .FirstOrDefaultAsync(le => le.Hash == logEntry.Hash);

            if (existingLogEntry == null)
            {
                dbContext.LogEntries.Add(logEntry);
            }
            else
            {
                // Update properties if necessary or handle according to your logic
                dbContext.Entry(existingLogEntry).CurrentValues.SetValues(logEntry);
            }

            await dbContext.SaveChangesAsync();
            return logEntry;
        }


        async Task<bool> ILogEntryRepository.DeleteLogEntry(int id)
        {
            var logEntry = await dbContext.LogEntries.FindAsync(id);
            if (logEntry == null)
            {
                return false;
            }

            dbContext.LogEntries.Remove(logEntry);
            int result = await dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}
