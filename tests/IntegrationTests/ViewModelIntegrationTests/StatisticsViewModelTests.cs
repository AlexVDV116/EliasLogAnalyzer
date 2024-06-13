using System.Collections.Specialized;
using System.ComponentModel;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services;
using EliasLogAnalyzer.MAUI.ViewModels;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IntegrationTests.ViewModelIntegrationTests;

public class StatisticsViewModelTests
{
    private readonly StatisticsViewModel _viewModel;
    private readonly LogDataSharingService _logDataSharingService;

    private static readonly LogTimestamp CommonTimestamp = new() { DateTime = DateTime.Now };
    private const string CommonData = "Common Data";

    public StatisticsViewModelTests()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddDebug());
        var logDataSharingLogger = loggerFactory.CreateLogger<LogDataSharingService>();

        _logDataSharingService = new LogDataSharingService(logDataSharingLogger);
        var settingsService = new SettingsService();
        var htmlGeneratorService = new HtmlGeneratorService(settingsService, _logDataSharingService);

        _viewModel = new StatisticsViewModel(_logDataSharingService, htmlGeneratorService);
    }

    [Fact]
    public void PinLogEntryCommand_Should_Call_PinLogEntry()
    {
        // Arrange
        var logEntry = new LogEntry();

        // Act
        _viewModel.PinLogEntryCommand.Execute(logEntry);

        // Assert
        Assert.Contains(logEntry, _logDataSharingService.PinnedLogEntries);
    }

    [Fact]
    public void OnMarkedLogEntryChanged_Should_Not_AnalyzeLogEntries_When_No_MarkedLogEntry()
    {
        // Arrange
        _logDataSharingService.MarkedLogEntry = null;

        // Act
        _viewModel.GetType().GetMethod("OnMarkedLogEntryChanged",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(_viewModel,
                new object[] { _logDataSharingService, new PropertyChangedEventArgs(nameof(_logDataSharingService.MarkedLogEntry)) });

        // Assert
        Assert.False(_viewModel.LogEntryMarked);
        Assert.True(_viewModel.NoLogEntryMarked);
        Assert.False(_viewModel.SortedLogEntriesByProbability.Any());

        Assert.False(string.IsNullOrEmpty(_viewModel.PieChartHtml));
        Assert.False(string.IsNullOrEmpty(_viewModel.TimelineHtml));
    }

    [Fact]
    public void OnMarkedLogEntryChanged_Should_UpdatePageState_And_AnalyzeLogEntries_And_GenerateCharts_When_MarkedLogEntry_Is_Not_Null()
    {
        // Arrange
        var logEntry1 = new LogEntry { LogTimeStamp = CommonTimestamp, Data = CommonData};
        var logEntry2 = new LogEntry { LogTimeStamp = CommonTimestamp, Data = CommonData };

        _logDataSharingService.LogEntries.Add(logEntry1);
        _logDataSharingService.LogEntries.Add(logEntry2);
        _logDataSharingService.MarkedLogEntry = logEntry1;

        // Act
        _viewModel.GetType().GetMethod("OnMarkedLogEntryChanged",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(_viewModel,
                new object[] { _logDataSharingService, new PropertyChangedEventArgs(nameof(_logDataSharingService.MarkedLogEntry)) });
        
        // Assert
        Assert.True(_viewModel.LogEntryMarked);
        Assert.False(_viewModel.NoLogEntryMarked);

        Assert.False(string.IsNullOrEmpty(_viewModel.PieChartHtml));
        Assert.False(string.IsNullOrEmpty(_viewModel.TimelineHtml));
    }

    [Fact]
    public void OnLogEntriesChanged_Should_AnalyzeLogEntries_And_GenerateCharts()
    {
        // Arrange
        var logEntry1 = new LogEntry { LogTimeStamp = CommonTimestamp, Data = CommonData};
        var logEntry2 = new LogEntry { LogTimeStamp = CommonTimestamp, Data = CommonData };

        _logDataSharingService.LogEntries.Add(logEntry1);
        _logDataSharingService.LogEntries.Add(logEntry2);
        _logDataSharingService.MarkedLogEntry = logEntry1;

        // Act
        _viewModel.GetType().GetMethod("OnLogEntriesChanged",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(_viewModel,
                new object[] { _logDataSharingService, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset) });

        // Assert
        Assert.False(string.IsNullOrEmpty(_viewModel.PieChartHtml));
        Assert.False(string.IsNullOrEmpty(_viewModel.TimelineHtml));
        Assert.False(string.IsNullOrEmpty(_viewModel.BarChartHtml));
    }
}
