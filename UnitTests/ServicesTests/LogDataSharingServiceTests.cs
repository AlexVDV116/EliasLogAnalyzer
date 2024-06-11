using System.Collections.ObjectModel;
using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace UnitTests;

public class LogDataSharingServiceTests
{
    private readonly LogDataSharingService _service;

    public LogDataSharingServiceTests()
    {
        Mock<ILogger<LogFileParserService>> mockLogger = new();
        _service = new LogDataSharingService(mockLogger.Object);
    }

    [Fact]
    public void AddLogFile_AddsLogFileAndEntries()
    {
        // Arrange
        var logEntries = new List<LogEntry>
        {
            new LogEntry { LogTimeStamp = new LogTimestamp { DateTime = DateTime.Now, Ticks = 1000 }, Hash = "entry1" },
            new LogEntry { LogTimeStamp = new LogTimestamp { DateTime = DateTime.Now, Ticks = 2000 }, Hash = "entry2" }
        };
        var logFile = new LogFile { FullPath = "path/to/logfile", LogEntries = logEntries };

        // Act
        _service.AddLogFile(logFile);

        // Assert
        Assert.Contains(logFile, _service.LogFiles);
        Assert.Contains(logEntries[0], _service.LogEntries);
        Assert.Contains(logEntries[1], _service.LogEntries);
    }

    [Fact]
    public void SortByProperty_SortsLogEntriesCorrectly()
    {
        // Arrange
        var logEntries = new ObservableCollection<LogEntry>
        {
            new LogEntry { LogTimeStamp = new LogTimestamp { DateTime = DateTime.Now, Ticks = 2000 }, Source = "SourceB" },
            new LogEntry { LogTimeStamp = new LogTimestamp { DateTime = DateTime.Now, Ticks = 1000 }, Source = "SourceA" }
        };
        _service.LogEntries = logEntries;

        // Act
        _service.SortByProperty("Source", ascending: true);

        // Assert
        Assert.Equal("SourceA", _service.LogEntries[0].Source);
        Assert.Equal("SourceB", _service.LogEntries[1].Source);
    }

    [Fact]
    public void UpdateFilter_FiltersLogEntriesCorrectly()
    {
        // Arrange
        var logEntries = new ObservableCollection<LogEntry>
        {
            new LogEntry { LogType = LogType.Error, Source = "SourceA" },
            new LogEntry { LogType = LogType.Warning, Source = "SourceB" }
        };
        _service.LogEntries = logEntries;
        _service.SelectedLogTypes = new ObservableCollection<LogType> { LogType.Error };

        // Act
        _service.UpdateFilter("SourceA");

        // Assert
        Assert.Single(_service.FilteredLogEntries);
        Assert.Equal("SourceA", _service.FilteredLogEntries[0].Source);
    }

    [Fact]
    public void PinLogEntry_TogglesPinState()
    {
        // Arrange
        var logEntry = new LogEntry { IsPinned = false };

        // Act
        _service.PinLogEntry(logEntry);

        // Assert
        Assert.True(logEntry.IsPinned);
        Assert.Contains(logEntry, _service.PinnedLogEntries);

        // Act
        _service.PinLogEntry(logEntry);

        // Assert
        Assert.False(logEntry.IsPinned);
        Assert.DoesNotContain(logEntry, _service.PinnedLogEntries);
    }

    [Fact]
    public void MarkLogEntry_TogglesMarkState()
    {
        // Arrange
        var logEntry1 = new LogEntry { IsMarked = false };
        var logEntry2 = new LogEntry { IsMarked = false };

        // Act
        _service.MarkLogEntry(logEntry1);

        // Assert
        Assert.True(logEntry1.IsMarked);
        Assert.Equal(logEntry1, _service.MarkedLogEntry);

        // Act
        _service.MarkLogEntry(logEntry2);

        // Assert
        Assert.False(logEntry1.IsMarked);
        Assert.True(logEntry2.IsMarked);
        Assert.Equal(logEntry2, _service.MarkedLogEntry);
    }

    [Fact]
    public void ClearAllLogs_ClearsAllCollections()
    {
        // Arrange
        var logEntries = new List<LogEntry>
        {
            new LogEntry { LogTimeStamp = new LogTimestamp { DateTime = DateTime.Now, Ticks = 1000 }, Hash = "entry1" },
            new LogEntry { LogTimeStamp = new LogTimestamp { DateTime = DateTime.Now, Ticks = 2000 }, Hash = "entry2" }
        };
        var logFile = new LogFile { FullPath = "path/to/logfile", LogEntries = logEntries };
        _service.AddLogFile(logFile);

        // Act
        _service.ClearAllLogs();

        // Assert
        Assert.Empty(_service.LogFiles);
        Assert.Empty(_service.LogEntries);
        Assert.Empty(_service.PinnedLogEntries);
        Assert.Empty(_service.SelectedLogEntries);
        Assert.Empty(_service.FilteredLogEntries);
        Assert.Null(_service.MarkedLogEntry);
    }
}