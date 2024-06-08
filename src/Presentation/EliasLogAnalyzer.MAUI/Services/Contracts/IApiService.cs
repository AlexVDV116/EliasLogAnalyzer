using EliasLogAnalyzer.BusinessLogic.Entities;

namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface IApiService
{
    Task<ApiResult> AddBugReportAsync(BugReport bugReport);

}
