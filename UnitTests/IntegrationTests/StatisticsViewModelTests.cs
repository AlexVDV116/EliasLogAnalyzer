using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using EliasLogAnalyzer.MAUI.ViewModels;
using Moq;
using System.Collections.Specialized;
using System.ComponentModel;
using Xunit;

namespace UnitTests.IntegrationTests;

public class StatisticsViewModelTests
{
    private readonly Mock<ILogDataSharingService> _logDataSharingServiceMock;
    private readonly Mock<ILogEntryAnalysisService> _logEntryAnalysisServiceMock;
    private readonly Mock<IHtmlGeneratorService> _htmlGeneratorServiceMock;
    private readonly StatisticsViewModel _viewModel;

    public StatisticsViewModelTests()
    {
        _logDataSharingServiceMock = new Mock<ILogDataSharingService>();
        _logEntryAnalysisServiceMock = new Mock<ILogEntryAnalysisService>();
        _htmlGeneratorServiceMock = new Mock<IHtmlGeneratorService>();

        _logDataSharingServiceMock.SetupGet(x => x.LogEntries).Returns([]);
        _logDataSharingServiceMock.SetupGet(x => x.SelectedLogEntries).Returns([]);
        _logDataSharingServiceMock.SetupGet(x => x.FilteredLogEntries).Returns([]);
        _logDataSharingServiceMock.SetupGet(x => x.MarkedLogEntry).Returns((LogEntry)null!);

        // Mock the INotifyPropertyChanged interface
        _logDataSharingServiceMock.As<INotifyPropertyChanged>();

        _viewModel = new StatisticsViewModel(
            _logDataSharingServiceMock.Object,
            _logEntryAnalysisServiceMock.Object,
            _htmlGeneratorServiceMock.Object
        );
    }

    [Fact]
    public void PinLogEntryCommand_Should_Call_PinLogEntry()
    {
        // Arrange
        var logEntry = new LogEntry();

        // Act
        _viewModel.PinLogEntryCommand.Execute(logEntry);

        // Assert
        _logDataSharingServiceMock.Verify(x => x.PinLogEntry(logEntry), Times.Once);
    }

    [Fact]
    public void OnMarkedLogEntryChanged_Should_Not_AnalyzeLogEntries_When_No_MarkedLogEntry()
    {
        // Arrange
        _logDataSharingServiceMock.SetupGet(x => x.MarkedLogEntry).Returns((LogEntry)null!);

        // Act
        _viewModel.GetType().GetMethod("OnMarkedLogEntryChanged",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(_viewModel,
                new object[]
                    { null!, new PropertyChangedEventArgs(nameof(_logDataSharingServiceMock.Object.MarkedLogEntry)) });

        // Assert
        Assert.False(_viewModel.LogEntryMarked);
        Assert.True(_viewModel.NoLogEntryMarked);

        _logEntryAnalysisServiceMock.Verify(x => x.AnalyzeLogEntries(), Times.Never);

        // Called twice because called in OnMarkedLogEntryChanged and in the constructor
        _htmlGeneratorServiceMock.Verify(x => x.GenerateTimeLineHtml(), Times.Exactly(2));
        _htmlGeneratorServiceMock.Verify(x => x.GeneratePieChartHtml(), Times.Exactly(2));
    }

    [Fact]
    public void
        OnMarkedLogEntryChanged_Should_UpdatePageState_And_AnalyzeLogEntries_And_GenerateCharts_When_MarkedLogEntry_Is_Not_Null()
    {
        // Arrange
        _logDataSharingServiceMock.SetupGet(x => x.MarkedLogEntry).Returns(new LogEntry());

        // Act
        _viewModel.GetType().GetMethod("OnMarkedLogEntryChanged",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(_viewModel,
                new object[]
                    { null!, new PropertyChangedEventArgs(nameof(_logDataSharingServiceMock.Object.MarkedLogEntry)) });

        // Assert
        Assert.True(_viewModel.LogEntryMarked);
        Assert.False(_viewModel.NoLogEntryMarked);
        _logEntryAnalysisServiceMock.Verify(x => x.AnalyzeLogEntries(), Times.Once);

        // Called twice because called in OnMarkedLogEntryChanged and in the constructor
        _htmlGeneratorServiceMock.Verify(x => x.GenerateTimeLineHtml(), Times.Exactly(2));
        _htmlGeneratorServiceMock.Verify(x => x.GeneratePieChartHtml(), Times.Exactly(2));
    }

    [Fact]
    public void OnLogEntriesChanged_Should_AnalyzeLogEntries_And_GenerateCharts()
    {
        // Arrange
        _logDataSharingServiceMock.SetupGet(x => x.MarkedLogEntry).Returns(new LogEntry());

        // Act
        _viewModel.GetType().GetMethod("OnLogEntriesChanged",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(_viewModel,
                new object[] { null!, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset) });

        // Assert
        _logEntryAnalysisServiceMock.Verify(x => x.AnalyzeLogEntries(), Times.Once);

        // Called twice because called in OnMarkedLogEntryChanged and in the constructor
        _htmlGeneratorServiceMock.Verify(x => x.GenerateTimeLineHtml(), Times.Exactly(2));
        _htmlGeneratorServiceMock.Verify(x => x.GeneratePieChartHtml(), Times.Exactly(2));
    }
}