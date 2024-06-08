namespace EliasLogAnalyzer.BusinessLogic.Entities;

public class ApiResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public static ApiResult Ok()
    {
        return new ApiResult { Success = true };
    }

    public static ApiResult Fail(string message)
    {
        return new ApiResult { Success = false, ErrorMessage = message };
    }
}
