using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace EliasLogAnalyzer.MAUI.Services;

public class ApiService(HttpClient httpClient, ILogger<ApiService> logger) : IApiService
{
    public async Task<ApiResult> CheckDatabaseConnectionAsync()
    {
        try
        {
            HttpResponseMessage response = await httpClient.GetAsync("api/Database/CheckConnection").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return ApiResult.Ok();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return ApiResult.Fail($"Failed to check database connection: {errorContent}");
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HttpRequestException occurred while checking the database connection.");
            return ApiResult.Fail("Unable to connect to the server. Please check your network connection.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while checking the database connection.");
            return ApiResult.Fail("An unexpected error occurred. Please try again later.");
        }
    }


    public async Task<ApiResult> AddBugReportAsync(BugReport bugReport)
    {
        try
        {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/BugReport", bugReport).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("BugReport successfully added.");
                return ApiResult.Ok();

            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogError("Failed to add BugReport. StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, errorContent);
                return ApiResult.Fail($"Failed to add BugReport: {errorContent}");
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HttpRequestException occurred while adding a BugReport.");
            return ApiResult.Fail("Unable to connect to the server. Please check your network connection.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while adding a BugReport.");
            return ApiResult.Fail("An unexpected error occurred. Please try again later.");
        }
    }
    
    
}
