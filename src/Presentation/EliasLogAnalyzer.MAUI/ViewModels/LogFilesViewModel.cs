using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class LogFilesViewModel : ObservableObject
{
    private readonly ILogDataSharingService _logDataSharingService;
    public ObservableCollection<LogFile> LoadedLogFiles => _logDataSharingService.LogFiles;
    
    private IEnumerable<LogFile> SelectedLogFiles => LoadedLogFiles.Where(file => file.IsSelected).ToList();
    
    public LogFilesViewModel(
        ILogDataSharingService logDataSharingService
    )
    {
        _logDataSharingService = logDataSharingService;
    }
    
    [RelayCommand]
    public void OpenFiles()
    {
        foreach (var file in SelectedLogFiles)
        {
            Console.WriteLine($"Selected File: {file.FileName}");
        }
    }
}