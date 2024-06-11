using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using EliasLogAnalyzer.MAUI.ViewModels;
using Moq;
using System.Collections.ObjectModel;
using Xunit;
using Microsoft.Extensions.Logging;

namespace UnitTests.IntegrationTests;

public class LogEntriesViewModelTests
{
    private readonly Mock<ILogFileLoaderService> _logFileLoaderServiceMock;
    private readonly Mock<ILogFileParserService> _logFileParserServiceMock;
    private readonly Mock<ILogDataSharingService> _logDataSharingServiceMock;
    private readonly Mock<ILogEntryAnalysisService> _logEntryAnalysisServiceMock;
    private readonly Mock<IHtmlGeneratorService> _htmlGeneratorServiceMock;
    private readonly Mock<IDialogService> _dialogServiceMock;
    private readonly LogEntriesViewModel _viewModel;

    public LogEntriesViewModelTests()
    {
        Mock<ILogger<LogEntriesViewModel>> loggerMock = new();
        _logFileLoaderServiceMock = new Mock<ILogFileLoaderService>();
        _logFileParserServiceMock = new Mock<ILogFileParserService>();
        _logDataSharingServiceMock = new Mock<ILogDataSharingService>();
        _logEntryAnalysisServiceMock = new Mock<ILogEntryAnalysisService>();
        _htmlGeneratorServiceMock = new Mock<IHtmlGeneratorService>();
        _dialogServiceMock = new Mock<IDialogService>();

        _logDataSharingServiceMock.SetupGet(x => x.LogEntries).Returns(new ObservableCollection<LogEntry>());
        _logDataSharingServiceMock.SetupGet(x => x.SelectedLogEntries).Returns(new ObservableCollection<object>());
        _logDataSharingServiceMock.SetupGet(x => x.FilteredLogEntries).Returns(new ObservableCollection<LogEntry>());
        _logDataSharingServiceMock.SetupGet(x => x.MarkedLogEntry).Returns((LogEntry)null!);

        _viewModel = new LogEntriesViewModel(
            loggerMock.Object,
            _logFileLoaderServiceMock.Object,
            _logFileParserServiceMock.Object,
            _logDataSharingServiceMock.Object,
            _logEntryAnalysisServiceMock.Object,
            _htmlGeneratorServiceMock.Object,
            _dialogServiceMock.Object
        );
    }
    
    [Fact]
    public void PinLogEntryCommand_Should_PinLogEntry()
    {
        // Arrange
        var logEntry = new LogEntry();

        // Act
        _viewModel.PinLogEntryCommand.Execute(logEntry);

        // Assert
        _logDataSharingServiceMock.Verify(x => x.PinLogEntry(logEntry), Times.Once);
    }

    [Fact]
    public void MarkLogEntryCommand_Should_MarkLogEntry_And_CalculateTimeDifference()
    {
        // Arrange
        var logEntry = new LogEntry();
        _logDataSharingServiceMock.SetupGet(x => x.MarkedLogEntry).Returns(logEntry);

        // Act
        _viewModel.MarkLogEntryCommand.Execute(logEntry);

        // Assert
        _logDataSharingServiceMock.Verify(x => x.MarkLogEntry(logEntry), Times.Once);
        _logEntryAnalysisServiceMock.Verify(x => x.CalcDiffTicks(), Times.Once);
    }

    [Fact]
    public void SelectionChangedCommand_Should_UpdateSelectedEntries()
    {
        // Arrange
        var logEntry1 = new LogEntry();
        var logEntry2 = new LogEntry();
        var logEntry3 = new LogEntry();

        _logDataSharingServiceMock.SetupGet(x => x.SelectedLogEntries).Returns(new ObservableCollection<object>
            { logEntry1, logEntry2, logEntry3 });

        // Act
        _viewModel.SelectionChangedCommand.Execute(null);

        // Assert
        Assert.Equal(3, _viewModel.SelectedLogEntries.Count);
        _htmlGeneratorServiceMock.Verify(x => x.ConvertDataToHtml(It.IsAny<LogEntry>()), Times.Exactly(3));
    }

    [Fact]
    public async Task DeleteLogEntriesCommand_Should_ClearAllLogs_When_Confirmed()
    {
        // Arrange
        _dialogServiceMock
            .Setup(x => x.ShowConfirmAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(true);

        // Act
        await _viewModel.DeleteLogEntriesCommand.ExecuteAsync(null);

        // Assert
        _logDataSharingServiceMock.Verify(x => x.ClearAllLogs(), Times.Once);
        Assert.Equal("0 / 0 👁️", _viewModel.SearchResultText);
        Assert.Equal("No log entries found, please load logfiles.", _viewModel.EmptyViewText);
    }

    [Fact]
    public async Task DeleteLogEntriesCommand_Should_Not_ClearLogs_When_Not_Confirmed()
    {
        // Arrange
        _dialogServiceMock
            .Setup(x => x.ShowConfirmAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(false);

        // Act
        await _viewModel.DeleteLogEntriesCommand.ExecuteAsync(null);

        // Assert
        _logDataSharingServiceMock.Verify(x => x.ClearAllLogs(), Times.Never);
    }
}