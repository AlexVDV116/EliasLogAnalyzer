using EliasLogAnalyzer.Domain.Entities;

namespace EliasLogAnalyzer.Persistence.Repositories.BugReports;

public interface IBugReportRepository
{
    Task<IEnumerable<BugReport>> GetAllBugReports();
    Task<BugReport?> GetBugReportById(int id);
    Task<BugReport> AddBugReportWithEntries(BugReport bugReport);
    Task<bool> DeleteBugReport(int id);
}
