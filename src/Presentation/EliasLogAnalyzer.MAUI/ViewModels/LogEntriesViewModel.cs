using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class LogEntriesViewModel : ObservableObject
{
    private readonly ILogDataSharingService _logDataSharingService;

    public ObservableCollection<LogEntry> LogEntries { get; } = [];

    [ObservableProperty]
    private LogEntry _selectedLogEntry;

    [ObservableProperty]
    private ObservableCollection<string> _selectedLogEntryData = [];

    public LogEntriesViewModel(
        ILogDataSharingService logDataSharingService
    )
    {
        _logDataSharingService = logDataSharingService;
        AggregateLogEntries();
    }

    private void AggregateLogEntries()
    {
        LogEntries.Clear();
        foreach (var file in _logDataSharingService.SelectedLogFiles)
        {
            foreach (var entry in file.LogEntries)
            {
                LogEntries.Add(entry);
            }
        }
    }

    // When the SelectedLogEntry changes, update the detail data
    partial void OnSelectedLogEntryChanged(LogEntry value)
    {
        UpdateSelectedLogEntryData();
    }

    private void UpdateSelectedLogEntryData()
    {

        SelectedLogEntryData.Clear();


        if (SelectedLogEntry != null)
        {

            SelectedLogEntryData.Add(SelectedLogEntry.Data);
        }
    }

}