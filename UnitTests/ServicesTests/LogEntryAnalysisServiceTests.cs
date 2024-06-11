using System.Collections.ObjectModel;
using System.Diagnostics;
using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Services;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests;

// Purpose: This file is used to test the LogEntryAnalysisService class in the EliasLogAnalyzer.MAUI project. The 
// LogEntryAnalysisService class is used to calculate the time delta and probability of log entries based on a marked log entry.
public class LogEntryAnalysisServiceTests
{
    private readonly LogEntryAnalysisService _service;
    private readonly ObservableCollection<LogEntry> _logEntries;
    private readonly LogEntry _markedLogEntry;
    public LogEntryAnalysisServiceTests(ITestOutputHelper output)
    {
        Mock<ILogDataSharingService> mockLogDataSharingService = new();
        _logEntries = new ObservableCollection<LogEntry>
        {
            new LogEntry
            {
                LogTimeStamp = new LogTimestamp { DateTime = DateTime.Parse("2023-06-01T12:00:00Z"), Ticks = 1000 },
                Data = "Stack trace line 1\nStack trace line 2"
            },
            new LogEntry
            {
                LogTimeStamp = new LogTimestamp { DateTime = DateTime.Parse("2023-06-01T12:00:01Z"), Ticks = 2000 },
                Data = "Stack trace line 1\nStack trace line 3"
            },
            new LogEntry
            {
                LogTimeStamp = new LogTimestamp { DateTime = DateTime.Parse("2023-06-01T12:00:06Z"), Ticks = 3000 },
                Data = "Stack trace line 4\nStack trace line 5"
            }
        };
        _markedLogEntry = new LogEntry
        {
            LogTimeStamp = new LogTimestamp { DateTime = DateTime.Parse("2023-06-01T12:00:00Z"), Ticks = 1000 },
            Data = "Stack trace line 1\nStack trace line 2"
        };

        mockLogDataSharingService.SetupGet(s => s.LogEntries).Returns(_logEntries);
        mockLogDataSharingService.SetupGet(s => s.MarkedLogEntry).Returns(_markedLogEntry);

        _service = new LogEntryAnalysisService(mockLogDataSharingService.Object);
    }

    [Fact]
    public void CalcDiffTicks_CalculatesCorrectTimeDelta()
    {
        // Act
        _service.CalcDiffTicks();

        // Assert
        Assert.Equal(0, _logEntries[0].TimeDelta);
        Assert.Equal(1000, _logEntries[1].TimeDelta); // 1-second difference
        Assert.Equal(6000, _logEntries[2].TimeDelta); // 6 seconds difference
    }

    [Fact]
    public void AnalyzeLogEntries_CalculatesExpectedProbabilities()
    {
        // Act
        _service.AnalyzeLogEntries();

        Assert.Equal(70, _logEntries[1].Probability); // 70% probability
        Assert.Equal(0, _logEntries[2].Probability); // 0% probability
    }
}