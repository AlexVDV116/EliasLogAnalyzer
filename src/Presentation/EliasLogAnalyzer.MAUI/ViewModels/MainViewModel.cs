using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ILogDataSharingService _logDataSharingService;
    private readonly ILogFileLoaderService _logFileLoaderService;
    private readonly ILogFileParserService _logFileParserService;
    
    public IRelayCommand LoadLogfilesCommand { get; }

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _loadingMessage;
    
    public ObservableCollection<LogFile> LogFiles => _logDataSharingService.LogFiles;

    public MainViewModel(
        ILogDataSharingService logDataSharingService,
        ILogFileLoaderService logFileLoaderService,
        ILogFileParserService logFileParserService
    )
    {
        _logDataSharingService = logDataSharingService;
        _logFileLoaderService = logFileLoaderService;
        _logFileParserService = logFileParserService;
        
        LoadLogfilesCommand = new RelayCommand(LoadLogFiles);
        LoadingMessage = "Open LogFiles...";
    }

    /// <summary>
    /// Asynchronously loads multiple log files and parses them concurrently. 
    /// Each parsing operation is performed in its own task, ensuring that the parsing process is concurrent
    /// and efficient. 
    /// </summary>
    private async void LoadLogFiles()
    {
        var fileResults = await _logFileLoaderService.LoadLogFilesAsync();
        
        IsLoading = true;
        LoadingMessage = "Please wait, parsing LogFiles...";

        // Create a list of tasks for parsing each new log file
        var parsingTasks = fileResults.Select(ParseLogFileAsync).ToList();
        
        // Await all tasks to complete
        var parsedLogFiles = await Task.WhenAll(parsingTasks);

        // Add the parsed log files to the shared service collection
        foreach (var logFile in parsedLogFiles)
        {
            _logDataSharingService.AddLogFile(logFile);
        }
        
        IsLoading = false; 
        LoadingMessage = "Open LogFiles...";
        await Shell.Current.GoToAsync("logentriesPage");
    }

    /// <summary>
    /// Asynchronously parses a single log file from a provided FileResult object. This method is designed
    /// to be called concurrently for multiple files, as each invocation operates independently, ensuring
    /// thread safety.
    /// </summary>
    /// <param name="fileResult">The file result from which the log file will be parsed.</param>
    /// <returns>A task that, when completed, returns a LogFile object containing all parsed entries.</returns>
    private async Task<LogFile> ParseLogFileAsync(FileResult fileResult)
    {
        var (logEntries, fileSize) = await _logFileParserService.ParseLogAsync(fileResult);
        var logFile = new LogFile
        {
            FileName = fileResult.FileName,
            FullPath = fileResult.FullPath,
            FileSize = fileSize,
            Computer = logEntries.FirstOrDefault()?.Computer ?? "Unknown",
            LogEntries = logEntries
        };

        foreach (var logEntry in logEntries)
        {
            logEntry.LogFile = logFile;
        }

        return logFile;
    }
}