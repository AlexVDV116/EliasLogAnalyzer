using EliasLogAnalyzer.Domain.Entities;

namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface IApiService
{
    Task<ApiResult> CheckDatabaseConnectionAsync();
    Task<ApiResult> AddBugReportAsync(BugReport bugReport);
}
