using EliasLogAnalyzer.Domain.Entities;
using Xunit;

namespace UnitTests.DomainClassesTests;

    public class LogEntryTests
    {
        [Fact]
        public void LogEntry_Should_Have_Default_Values()
        {
            // Arrange & Act
            var logEntry = new LogEntry();

            // Assert
            Assert.Equal(0, logEntry.LogEntryId);
            Assert.Equal(string.Empty, logEntry.Hash);
            Assert.Equal(LogType.None, logEntry.LogType);
            Assert.Equal(string.Empty, logEntry.ThreadNameOrNumber);
            Assert.Equal(string.Empty, logEntry.SourceLocation);
            Assert.Equal(string.Empty, logEntry.Source);
            Assert.Equal(string.Empty, logEntry.Category);
            Assert.Equal(int.MaxValue, logEntry.EventId);
            Assert.Equal(string.Empty, logEntry.User);
            Assert.Equal(string.Empty, logEntry.Computer);
            Assert.Equal(string.Empty, logEntry.Description);
            Assert.Equal(string.Empty, logEntry.Data);
            Assert.False(logEntry.IsPinned);
            Assert.False(logEntry.IsMarked);
            Assert.Null(logEntry.TimeDelta);
            Assert.Null(logEntry.Probability);
            Assert.NotNull(logEntry.BugReports);
            Assert.Empty(logEntry.BugReports);
            Assert.NotNull(logEntry.BugReportLogEntries);
            Assert.Empty(logEntry.BugReportLogEntries);
            Assert.NotNull(logEntry.LogFile);
        }

        [Fact]
        public void Setting_IsPinned_Should_Raise_PropertyChanged()
        {
            // Arrange
            var logEntry = new LogEntry();
            bool propertyChangedRaised = false;
            logEntry.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(LogEntry.IsPinned))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            logEntry.IsPinned = true;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.True(logEntry.IsPinned);
        }

        [Fact]
        public void Setting_IsMarked_Should_Raise_PropertyChanged()
        {
            // Arrange
            var logEntry = new LogEntry();
            bool propertyChangedRaised = false;
            logEntry.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(LogEntry.IsMarked))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            logEntry.IsMarked = true;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.True(logEntry.IsMarked);
        }

        [Fact]
        public void Setting_TimeDelta_Should_Raise_PropertyChanged()
        {
            // Arrange
            var logEntry = new LogEntry();
            bool propertyChangedRaised = false;
            logEntry.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(LogEntry.TimeDelta))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            logEntry.TimeDelta = 100;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal(100, logEntry.TimeDelta);
        }

        [Fact]
        public void Setting_Probability_Should_Raise_PropertyChanged()
        {
            // Arrange
            var logEntry = new LogEntry();
            bool propertyChangedRaised = false;
            logEntry.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(LogEntry.Probability))
                {
                    propertyChangedRaised = true;
                }
            };

            // Act
            logEntry.Probability = 80;

            // Assert
            Assert.True(propertyChangedRaised);
            Assert.Equal(80, logEntry.Probability);
        }
    }

