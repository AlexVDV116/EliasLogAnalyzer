using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Services;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests;

// Purpose: This file is used to test the HashService class in the EliasLogAnalyzer.MAUI project. The HashService class
// is used to generate SHA256 hashes for log entries and log files. The GenerateLogEntryHash method generates a hash
// for a log entry, and the GenerateLogFileHash method generates a hash for a log file. The tests in this file verify
// that the hashes generated by the HashService class are consistent.
public class HashServiceTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HashService _hashService;

    public HashServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _hashService = new HashService();
    }

    [Fact]
    public void GenerateLogEntryHash_ReturnsConsistentHash()
    {
        // Arrange
        var logEntry = new LogEntry
        {
            LogTimeStamp = new LogTimestamp { DateTime = DateTime.Parse("2024-06-11T12:34:56Z") },
            LogType = LogType.Information,
            ThreadNameOrNumber = "MainThread",
            SourceLocation = "Main.cs:45",
            Source = "UnitTest",
            Category = "TestRun",
            EventId = 101,
            User = "tester",
            Computer = "localhost",
            Description = "A test log entry",
            Data = "Some data related to the log entry"
        };

        // Act
        var hash = _hashService.GenerateLogEntryHash(logEntry);

        // Assert
        Assert.NotNull(hash);
        Assert.Equal("86f1300d291646100bf6dd66b448a0066c51ddd3c6ac87afe3ef92b499a4eaee", hash); 
    }

    [Fact]
    public void GenerateLogFileHash_ReturnsConsistentHash()
    {
        // Arrange
        var logEntries = new List<LogEntry>
        {
            new LogEntry { LogTimeStamp = new LogTimestamp { DateTime = DateTime.Parse("2024-06-11T12:34:56Z") }, LogType = LogType.Error, Data = "Error occurred" },
            new LogEntry { LogTimeStamp = new LogTimestamp { DateTime = DateTime.Parse("2023-06-11T12:34:56Z") }, LogType = LogType.Warning, Data = "Warning issued" }
        };
        var logFile = new LogFile
        {
            FileName = "log2021.txt",
            Computer = "server01",
            LogEntries = logEntries
        };

        // Act
        var hash = _hashService.GenerateLogFileHash(logFile);

        // Assert
        Assert.NotNull(hash);
        Assert.Equal("10538463662c0500a9652b8323f3936db53a8d357b27485527d4c9998d1b7b54", hash); 
    }
}
