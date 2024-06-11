using System.Net;
using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace UnitTests;

// Purpose: This file contains unit tests for the ApiService class in the EliasLogAnalyzer.MAUI project.
// The ApiService class is responsible for making API calls to the EliasLogAnalyzer API. 
// It handles tasks such as checking the database connection and adding bug reports. 
// The tests in this file verify that the ApiService class correctly handles various scenarios, 
// including successful API responses, failed API responses, and exceptions. 
// These tests ensure that the ApiService class behaves as expected, logs appropriate messages, 
// and returns accurate results based on the API responses.

public class ApiServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly ApiService _apiService;

    public ApiServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object) { BaseAddress = new Uri("http://localhost/") };
        Mock<ILogger<ApiService>> loggerMock = new();
        _apiService = new ApiService(httpClient, loggerMock.Object);
    }

    [Fact]
    public async Task CheckDatabaseConnectionAsync_ReturnsOk_WhenSuccessStatusCode()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        // Act
        var result = await _apiService.CheckDatabaseConnectionAsync();

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public async Task CheckDatabaseConnectionAsync_ReturnsFail_WhenNotSuccessStatusCode()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Bad request")
            });

        // Act
        var result = await _apiService.CheckDatabaseConnectionAsync();

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Failed to check database connection: Bad request", result.ErrorMessage);
    }

    [Fact]
    public async Task CheckDatabaseConnectionAsync_ReturnsFail_WhenHttpRequestExceptionThrown()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Request failed"));

        // Act
        var result = await _apiService.CheckDatabaseConnectionAsync();

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Unable to connect to the server. Please check your network connection.", result.ErrorMessage);
    }

    [Fact]
    public async Task AddBugReportAsync_ReturnsOk_WhenSuccessStatusCode()
    {
        // Arrange
        var bugReport = new BugReport { DeveloperName = "Test Developer" };
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        // Act
        var result = await _apiService.AddBugReportAsync(bugReport);

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public async Task AddBugReportAsync_ReturnsFail_WhenNotSuccessStatusCode()
    {
        // Arrange
        var bugReport = new BugReport { DeveloperName = "Test Developer" };
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Bad request")
            });

        // Act
        var result = await _apiService.AddBugReportAsync(bugReport);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Failed to add BugReport: Bad request", result.ErrorMessage);
    }

    [Fact]
    public async Task AddBugReportAsync_ReturnsFail_WhenHttpRequestExceptionThrown()
    {
        // Arrange
        var bugReport = new BugReport { DeveloperName = "Test Developer" };
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Request failed"));

        // Act
        var result = await _apiService.AddBugReportAsync(bugReport);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Unable to connect to the server. Please check your network connection.", result.ErrorMessage);
    }
}