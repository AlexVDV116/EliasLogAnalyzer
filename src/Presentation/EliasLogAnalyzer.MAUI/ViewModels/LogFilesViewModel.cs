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
    public ObservableCollection<LogFile> SelectedLogFiles => _logDataSharingService.SelectedLogFiles;
    
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
    
    public LogFilesViewModel(ILogDataSharingService logDataSharingService)
    {
        _logDataSharingService = logDataSharingService;
    }
    
    [RelayCommand]
    private async Task OpenFiles()
    {
        foreach (var file in LoadedLogFiles.Where(file => file.IsSelected))
        {
            _logDataSharingService.AddLogFileToSelected(file);
        }
        await Shell.Current.GoToAsync("logentriesPage");
    }
    
}