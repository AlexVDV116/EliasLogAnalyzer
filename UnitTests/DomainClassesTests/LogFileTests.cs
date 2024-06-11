using EliasLogAnalyzer.BusinessLogic.Entities;
using Xunit;

namespace UnitTests.DomainClassesTests;

public class LogFileTests
{
    [Fact]
    public void LogFile_Should_Have_Default_Values()
    {
        // Arrange & Act
        var logFile = new LogFile();

        // Assert
        Assert.Equal(0, logFile.LogFileId);
        Assert.Equal(string.Empty, logFile.Hash);
        Assert.Equal(string.Empty, logFile.FileName);
        Assert.Equal(string.Empty, logFile.FullPath);
        Assert.Equal(string.Empty, logFile.Computer);
        Assert.False(logFile.IsSelected);
        Assert.NotNull(logFile.LogEntries);
        Assert.Empty(logFile.LogEntries);
    }

    [Fact]
    public void Setting_IsSelected_Should_Raise_PropertyChanged()
    {
        // Arrange
        var logFile = new LogFile();
        var propertyChangedRaised = false;
        logFile.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(LogFile.IsSelected))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        logFile.IsSelected = true;

        // Assert
        Assert.True(propertyChangedRaised);
        Assert.True(logFile.IsSelected);
    }

    [Fact]
    public void Adding_LogEntry_Should_Add_To_LogEntries()
    {
        // Arrange
        var logFile = new LogFile();
        var logEntry = new LogEntry { LogEntryId = 1, Description = "Test Log Entry" };

        // Act
        logFile.LogEntries.Add(logEntry);

        // Assert
        Assert.Contains(logEntry, logFile.LogEntries);
        Assert.Single(logFile.LogEntries);
    }
}