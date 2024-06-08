using EliasLogAnalyzer.BusinessLogic.Entities;

namespace EliasLogAnalyzer.API.Repositories.BugReports;

public interface IBugReportRepository
{
    Task<IEnumerable<BugReport>> GetAllBugReports();
    Task<BugReport?> GetBugReportById(int id);
    Task<BugReport> AddBugReportWithEntries(BugReport bugReport);
    Task<bool> DeleteBugReport(int id);
}
