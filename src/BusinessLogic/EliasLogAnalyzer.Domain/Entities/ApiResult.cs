namespace EliasLogAnalyzer.Domain.Entities;

public class ApiResult
{
    public bool Success { get; private init; }
    public string ErrorMessage { get; private init; } = string.Empty;

    public static ApiResult Ok()
    {
        return new ApiResult { Success = true };
    }

    public static ApiResult Fail(string message)
    {
        return new ApiResult { Success = false, ErrorMessage = message };
    }
}
