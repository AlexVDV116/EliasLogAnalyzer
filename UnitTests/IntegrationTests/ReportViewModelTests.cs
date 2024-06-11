using System.Collections.ObjectModel;
using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using EliasLogAnalyzer.MAUI.ViewModels;
using Moq;
using Xunit;

namespace UnitTests.IntegrationTests;

// Purpose: This file is used to test the ReportViewModel class in the EliasLogAnalyzer.MAUI project. The ReportViewModel
// class is used to submit bug reports to the EliasLogAnalyzer API. The tests in this file verify that the SubmitCommand
// method in the ReportViewModel class correctly submits bug reports to the API.

public class ReportViewModelTests
{
    private readonly Mock<ILogDataSharingService> _logDataSharingMock;
    private readonly Mock<IDialogService> _dialogServiceMock;
    private readonly Mock<IApiService> _apiServiceMock;
    private readonly ReportViewModel _viewModel;

    public ReportViewModelTests()
    {
        _logDataSharingMock = new Mock<ILogDataSharingService>();
        _dialogServiceMock = new Mock<IDialogService>();
        _apiServiceMock = new Mock<IApiService>();

        // Ensure PinnedLogEntries is initialized to prevent NullReferenceException
        var pinnedLogEntries = new ObservableCollection<LogEntry>();
        _logDataSharingMock.Setup(m => m.PinnedLogEntries).Returns(pinnedLogEntries);

        _viewModel = new ReportViewModel(_logDataSharingMock.Object, _dialogServiceMock.Object, _apiServiceMock.Object);
    }

    [Fact]
    public async Task SubmitCommand_Should_Invoke_ApiService_And_Return_Success()
    {
        // Arrange
        _logDataSharingMock.Setup(m => m.PinnedLogEntries).Returns([]);
        _apiServiceMock.Setup(x => x.AddBugReportAsync(It.IsAny<BugReport>()))
            .ReturnsAsync(ApiResult.Ok()); 
        
        _viewModel.DeveloperName = "John Doe";
        _viewModel.Severity = "High";
        _viewModel.Analysis = "Detailed analysis";
        _viewModel.Recommendation = "Follow-up needed";

        // Act
        await _viewModel.SubmitCommand.ExecuteAsync(null);

        // Assert
        _dialogServiceMock.Verify(d => d.ShowMessage("Success", "Report successfully submitted."), Times.Once);
    }
    
    [Fact]
    public async Task SubmitCommand_ShouldNot_Invoke_ApiService_When_Form_Is_Invalid()
    {
        // Arrange - setting up an invalid state for the form
        _viewModel.DeveloperName = ""; 
        _viewModel.Severity = "";
        _viewModel.Analysis = "";
        _viewModel.Recommendation = "";

        // Act
        await _viewModel.SubmitCommand.ExecuteAsync(null);

        // Assert
        _apiServiceMock.Verify(api => api.AddBugReportAsync(It.IsAny<BugReport>()), Times.Never);
        _dialogServiceMock.Verify(dialog => dialog.ShowMessage("Validation Error", "Please fill in all required fields."), Times.Once);
    }
}