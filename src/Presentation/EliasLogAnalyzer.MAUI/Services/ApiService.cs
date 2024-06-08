using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using System.Net.Http.Json;


namespace EliasLogAnalyzer.MAUI.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ApiResult> AddBugReportAsync(BugReport bugReport)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/BugReport", bugReport).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("BugReport successfully added.");
                return ApiResult.Ok();

            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                _logger.LogError("Failed to add BugReport. StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, errorContent);
                return ApiResult.Fail($"Failed to add BugReport: {errorContent}");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HttpRequestException occurred while adding a BugReport.");
            return ApiResult.Fail("Unable to connect to the server. Please check your network connection.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while adding a BugReport.");
            return ApiResult.Fail("An unexpected error occurred. Please try again later.");
        }
    }
}
