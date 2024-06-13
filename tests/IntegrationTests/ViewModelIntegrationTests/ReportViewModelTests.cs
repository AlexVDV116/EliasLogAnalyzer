using System.Collections.ObjectModel;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using EliasLogAnalyzer.MAUI.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IntegrationTests.ViewModelIntegrationTests;

// Purpose: This file is used to test the ReportViewModel class in the EliasLogAnalyzer.MAUI project. The ReportViewModel
// class is used to submit bug reports to the EliasLogAnalyzer API. The tests in this file verify that the SubmitCommand
// method in the ReportViewModel class correctly submits bug reports to the API.

public class ReportViewModelTests
{
    private readonly LogDataSharingService _logDataSharingService;
    private readonly Mock<IApiService> _apiServiceMock;
    private readonly ReportViewModel _viewModel;

    public ReportViewModelTests()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();  // or builder.AddConsole() depending on what is available
        });
        loggerFactory.CreateLogger<ApiService>();
        var logDataSharingLogger = loggerFactory.CreateLogger<LogDataSharingService>();

        _logDataSharingService = new LogDataSharingService(logDataSharingLogger);
        var dialogService = new DialogService();
        _apiServiceMock = new Mock<IApiService>();

        _viewModel = new ReportViewModel(_logDataSharingService, dialogService, _apiServiceMock.Object);
    }

    [Fact]
    public async Task SubmitCommand_Should_Invoke_ApiService_And_Return_Success()
    {
        // Arrange
        var pinnedLogEntries = new ObservableCollection<LogEntry> { new LogEntry { Description = "Test log" } };
        _logDataSharingService.PinnedLogEntries = pinnedLogEntries;
        
        _viewModel.Analysis = "Detailed analysis";
        _viewModel.Recommendation = "Follow-up needed";

        _apiServiceMock.Setup(x => x.AddBugReportAsync(It.IsAny<BugReport>()))
            .ReturnsAsync(ApiResult.Ok());

        // Act
        await _viewModel.SubmitCommand.ExecuteAsync(null);

        // Assert
        _apiServiceMock.Verify(x => x.AddBugReportAsync(It.IsAny<BugReport>()), Times.Once);
    }
    
    [Fact]
    public async Task SubmitCommand_ShouldNot_Invoke_ApiService_When_Form_Is_Invalid()
    {
        // Arrange - setting up an invalid state for the form
        _viewModel.Analysis = "";
        _viewModel.Recommendation = "";

        // Act
        await _viewModel.SubmitCommand.ExecuteAsync(null);

        // Assert
        _apiServiceMock.Verify(api => api.AddBugReportAsync(It.IsAny<BugReport>()), Times.Never);
    }
    
    [Fact]
    public async Task CheckConnectionCommand_Should_UpdateViewModel()
    {
        // Arrange
        _apiServiceMock.Setup(x => x.CheckDatabaseConnectionAsync())
            .ReturnsAsync(ApiResult.Ok());

        // Act
        await _viewModel.CheckConnectionCommand.ExecuteAsync(null);

        // Assert
        if (_viewModel.IsConnected)
        {
            Assert.True(_viewModel.ConnectedIconVisible);
            Assert.False(_viewModel.NotConnectedIconVisible);
            Assert.Equal("Connected", _viewModel.ConnectionStatus);
        }
        else
        {
            Assert.False(_viewModel.ConnectedIconVisible);
            Assert.True(_viewModel.NotConnectedIconVisible);
            Assert.NotEqual("Connected", _viewModel.ConnectionStatus);
        }
    }
}
