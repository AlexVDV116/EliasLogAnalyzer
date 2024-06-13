using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services;
using EliasLogAnalyzer.MAUI.ViewModels;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IntegrationTests.ViewModelIntegrationTests;

public class LogEntriesViewModelTests
{
    private readonly LogEntriesViewModel _viewModel;
    private readonly LogDataSharingService _logDataSharingService; 

    public LogEntriesViewModelTests()
    {        
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
        });
        var logEntriesLogger = loggerFactory.CreateLogger<LogEntriesViewModel>();
        var logFileParserLogger = loggerFactory.CreateLogger<LogFileParserService>();
        var logDataSharingLogger = loggerFactory.CreateLogger<LogDataSharingService>();
        
        var logFileLoaderService = new LogFileLoaderService(); 
        var hashService = new HashService(); 
        var settingsService = new SettingsService(); 
        var logFileParserService = new LogFileParserService(logFileParserLogger, hashService); 
        _logDataSharingService = new LogDataSharingService(logDataSharingLogger); 
        var logEntryAnalysisService = new LogEntryAnalysisService(_logDataSharingService); 
        var htmlGeneratorService = new HtmlGeneratorService(settingsService, _logDataSharingService); 
        var dialogService = new DialogService();

        _viewModel = new LogEntriesViewModel(
            logEntriesLogger,
            logFileLoaderService,
            logFileParserService,
            _logDataSharingService,
            logEntryAnalysisService,
            htmlGeneratorService,
            dialogService
        );
    }
    
    [Fact]
    public async Task ViewModel_Integration_Test()
    {
        var logEntry = new LogEntry();

        // Test Pinning
        _viewModel.PinLogEntryCommand.Execute(logEntry);
        Assert.Contains(logEntry, _logDataSharingService.PinnedLogEntries);

        // Test Marking
        _viewModel.MarkLogEntryCommand.Execute(logEntry);
        Assert.Equal(logEntry, _logDataSharingService.MarkedLogEntry);

        // Test Deleting
        await _viewModel.DeleteLogEntriesCommand.ExecuteAsync(null);
        Assert.Empty(_logDataSharingService.LogEntries);

        // Test Selection Changed
        _viewModel.SelectedLogEntries.Add(logEntry);
        _viewModel.SelectionChangedCommand.Execute(null);
        Assert.Contains(logEntry, _viewModel.SelectedLogEntries);
    }
}
