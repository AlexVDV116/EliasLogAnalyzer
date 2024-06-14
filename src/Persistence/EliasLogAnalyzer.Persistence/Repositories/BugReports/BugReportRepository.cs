using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EliasLogAnalyzer.Persistence.Repositories.BugReports;

public class BugReportRepository(
    EliasLogAnalyzerDbContext dbContext,
    ILogger<BugReportRepository> logger)
    : IBugReportRepository
{

    public async Task<IEnumerable<BugReport>> GetAllBugReports()
    {
        return await dbContext.BugReports.ToListAsync();
    }

    public async Task<BugReport?> GetBugReportById(int id)
    {
        return await dbContext.BugReports.FindAsync(id);
    }

    public async Task<BugReport> AddBugReportWithEntries(BugReport bugReport)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            // List to hold processed log entries
            List<LogEntry> processedLogEntries = [];

            foreach (var logEntry in bugReport.LogEntriesToInclude)
            {

                // Handle the LogFile to ensure it's not duplicated
                var existingLogFile = await dbContext.LogFiles
                                                    .FirstOrDefaultAsync(lf => lf.Hash == logEntry.LogFile.Hash);
                if (existingLogFile == null)
                {
                    dbContext.LogFiles.Add(logEntry.LogFile);
                    await dbContext.SaveChangesAsync(); // This populates the LogFileId needed for the LogEntry
                }
                else
                {
                    logEntry.LogFile = existingLogFile; // Reuse existing LogFile
                }

                // Handle the LogEntry
                var existingLogEntry = await dbContext.LogEntries
                                                     .FirstOrDefaultAsync(le => le.Hash == logEntry.Hash);
                if (existingLogEntry == null)
                {
                    dbContext.LogEntries.Add(logEntry);
                    await dbContext.SaveChangesAsync();  // Ensures LogEntryId is populated
                    logEntry.LogFile.LogEntries.Add(logEntry); // Ensure relationship is established
                    processedLogEntries.Add(logEntry); // Add to processed list
                }
                else
                {
                    // Use existing LogEntry
                    processedLogEntries.Add(existingLogEntry);
                }
            }

            // Replace the list with processed entries
            bugReport.LogEntriesToInclude = processedLogEntries;

            foreach (var entry in processedLogEntries)
            {
                // Manage BugReportLogEntries relationship
                var linkExists = bugReport.BugReportLogEntries.Any(ble => ble.LogEntryId == entry.LogEntryId);
                if (!linkExists)
                {
                    bugReport.BugReportLogEntries.Add(new BugReportLogEntry
                    {
                        BugReportId = bugReport.BugReportId,
                        LogEntryId = entry.LogEntryId
                    });
                }
            }

            // Add the BugReport after ensuring all related entities are correctly handled
            dbContext.BugReports.Add(bugReport);
            await dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
            return bugReport;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Error adding BugReport with ID {BugReportId}: {Message}", bugReport.BugReportId, ex.Message);
            throw;
        }
    }

    public async Task<bool> DeleteBugReport(int id)
    {
        var report = await dbContext.BugReports.FindAsync(id);
        if (report == null)
        {
            return false;
        }

        dbContext.BugReports.Remove(report);
        int result = await dbContext.SaveChangesAsync();
        return result > 0;
    }
}
