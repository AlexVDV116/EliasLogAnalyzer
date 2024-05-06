using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class LogFilesViewModel : ObservableObject
{
    private readonly ILogDataSharingService _logDataSharingService;
    private bool _selectAll;
    
    public ObservableCollection<LogFile> LoadedLogFiles => _logDataSharingService.LogFiles;
    
    private IEnumerable<LogFile> SelectedLogFiles => LoadedLogFiles.Where(file => file.IsSelected).ToList();
    
    public bool SelectAll
    {
        get => _selectAll;
        set
        {
            SetProperty(ref _selectAll, value);
            foreach (var file in LoadedLogFiles)
            {
                file.IsSelected = _selectAll;
            }
        }
    }
    
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