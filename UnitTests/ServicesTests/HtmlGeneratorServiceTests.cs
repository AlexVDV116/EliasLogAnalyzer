using System.Collections.ObjectModel;
using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Resources;
using EliasLogAnalyzer.MAUI.Services;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Moq;
using Xunit;

namespace UnitTests;

// Purpose: This file contains unit tests for the HtmlGeneratorService class in the EliasLogAnalyzer.MAUI project.
// The HtmlGeneratorService class generates HTML representations of log entries, including converting log entry data to HTML and creating timeline and pie chart visualizations.
// The tests in this file verify that the HtmlGeneratorService class correctly generates HTML output based on different themes and log entry data.


public class HtmlGeneratorServiceTests
{
    private readonly Mock<ISettingsService> _settingsServiceMock;
    private readonly Mock<ILogDataSharingService> _logDataSharingServiceMock;
    private readonly HtmlGeneratorService _htmlGeneratorService;

    public HtmlGeneratorServiceTests()
    {
        _settingsServiceMock = new Mock<ISettingsService>();
        _logDataSharingServiceMock = new Mock<ILogDataSharingService>();
        _htmlGeneratorService = new HtmlGeneratorService(_settingsServiceMock.Object, _logDataSharingServiceMock.Object);
    }

    [Fact]
    public void ConvertDataToHtml_GeneratesCorrectHtmlForLightTheme()
    {
        // Arrange
        _settingsServiceMock.Setup(s => s.AppTheme).Returns(Theme.Light);
        var logEntry = new LogEntry
        {
            LogTimeStamp = new LogTimestamp { DateTime = new DateTime(2023, 6, 1, 12, 0, 0) },
            LogType = LogType.Error,
            Source = "SourceA",
            User = "UserA",
            Computer = "ComputerA",
            Description = "Test description",
            Data = "Test data"
        };

        // Act
        var html = _htmlGeneratorService.ConvertDataToHtml(logEntry);

        // Assert
        Assert.Contains("#f9f9f9", html); // Background color
        Assert.Contains("#333", html); // Text color
        Assert.Contains("Test description", html);
        Assert.Contains("Test data", html);
    }

    [Fact]
    public void ConvertDataToHtml_GeneratesCorrectHtmlForDarkTheme()
    {
        // Arrange
        _settingsServiceMock.Setup(s => s.AppTheme).Returns(Theme.Dark);
        var logEntry = new LogEntry
        {
            LogTimeStamp = new LogTimestamp { DateTime = new DateTime(2023, 6, 1, 12, 0, 0) },
            LogType = LogType.Error,
            Source = "SourceA",
            User = "UserA",
            Computer = "ComputerA",
            Description = "Test description",
            Data = "Test data"
        };

        // Act
        var html = _htmlGeneratorService.ConvertDataToHtml(logEntry);

        // Assert
        Assert.Contains("#333", html); // Background color
        Assert.Contains("#f9f9f9", html); // Text color
        Assert.Contains("Test description", html);
        Assert.Contains("Test data", html);
    }

    [Fact]
    public void GenerateTimeLineHtml_GeneratesCorrectHtmlForNoLogEntries()
    {
        // Arrange
        _logDataSharingServiceMock.Setup(s => s.LogEntries).Returns(new ObservableCollection<LogEntry>());

        // Act
        var html = _htmlGeneratorService.GenerateTimeLineHtml();

        // Assert
        Assert.Contains("var option = {", html); // Chart options
        Assert.Contains("'No Data'", html); // No data case
    }

    [Fact]
    public void GenerateTimeLineHtml_GeneratesCorrectHtmlForLogEntries()
    {
        // Arrange
        var logEntries = new ObservableCollection<LogEntry>
        {
            new LogEntry
            {
                LogTimeStamp = new LogTimestamp { DateTime = new DateTime(2023, 6, 1, 12, 0, 0) },
                LogType = LogType.Error
            },
            new LogEntry
            {
                LogTimeStamp = new LogTimestamp { DateTime = new DateTime(2023, 6, 1, 12, 1, 0) },
                LogType = LogType.Warning
            }
        };
        _logDataSharingServiceMock.Setup(s => s.LogEntries).Returns(logEntries);

        // Act
        var html = _htmlGeneratorService.GenerateTimeLineHtml();

        // Assert
        Assert.Contains("var option = {", html); // Chart options
        Assert.Contains("2023-06-01 12:00:00", html); // Log entry timestamps
        Assert.Contains("Error", html); // Log entry types
        Assert.Contains("Warning", html); // Log entry types
    }

    [Fact]
    public void GeneratePieChartHtml_GeneratesCorrectHtmlForNoLogEntries()
    {
        // Arrange
        _logDataSharingServiceMock.Setup(s => s.LogEntries).Returns(new ObservableCollection<LogEntry>());

        // Act
        var html = _htmlGeneratorService.GeneratePieChartHtml();

        // Assert
        Assert.Contains("var option = {", html); // Chart options
        Assert.Contains("'No Data'", html); // No data case
    }

    [Fact]
    public void GeneratePieChartHtml_GeneratesCorrectHtmlForLogEntries()
    {
        // Arrange
        var logEntries = new ObservableCollection<LogEntry>
        {
            new LogEntry { LogType = LogType.Error },
            new LogEntry { LogType = LogType.Warning },
            new LogEntry { LogType = LogType.Information },
            new LogEntry { LogType = LogType.Debug }
        };
        _logDataSharingServiceMock.Setup(s => s.LogEntries).Returns(logEntries);

        // Act
        var html = _htmlGeneratorService.GeneratePieChartHtml();

        // Assert
        Assert.Contains("var option = {", html); // Chart options
        Assert.Contains("Error", html); // Log entry types
        Assert.Contains("Warning", html); // Log entry types
        Assert.Contains("Information", html); // Log entry types
        Assert.Contains("Debug", html); // Log entry types
    }
}