using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class LogEntriesViewModel : ObservableObject
{
    private readonly ILogDataSharingService _logDataSharingService;
    
    [ObservableProperty]
    private ObservableCollection<string> _selectedLogEntryData = [];

    [ObservableProperty]
    private LogEntry _selectedLogEntry;
    
    [ObservableProperty]
    private string _currentSortProperty;
    
    [ObservableProperty]
    private string _sortHeader;
    
    [ObservableProperty]
    private string _currentSortDirection;
    
    [ObservableProperty]
    private string _ascendingText = "✓ Ascending";
    
    [ObservableProperty]
    private string _descendingText = "Descending";
    
    private bool _ascending = true;
    
    private bool Ascending
    {
        get => _ascending;
        set
        {
            if (SetProperty(ref _ascending, value))
            {
                AscendingText = value ? "✓ Ascending" : "Ascending";
                DescendingText = !value ? "✓ Descending" : "Descending";
            }
        }
    }
    
    public ObservableCollection<LogEntry> LogEntries { get; set; } = [];

    public LogEntriesViewModel(
        ILogDataSharingService logDataSharingService
    )
    {
        _logDataSharingService = logDataSharingService;
        AggregateLogEntries();
        SortByDateTime();
    }
    
    private void UpdateSortHeader()
    {
        SortHeader = $"Sorting: \n{CurrentSortProperty} {(Ascending ? "Asc" : "Desc")}";
    }
    
    [RelayCommand]
    public void SetSortOrderAscending()
    {
        Ascending = true;
    }
    
    [RelayCommand]
    public void SetSortOrderDescending()
    {
        Ascending = false;
    }
    
    [RelayCommand]
    private void SortByDateTime()
    {
        if (Ascending)
        {
            LogEntries = new ObservableCollection<LogEntry>(
                LogEntries.OrderBy(x => x.LogTimeStamp.DateTime)
                    .ThenBy(x => x.LogTimeStamp.Ticks));
        }
        else
        {
            LogEntries = new ObservableCollection<LogEntry>(
                LogEntries.OrderByDescending(x => x.LogTimeStamp.DateTime)
                    .ThenByDescending(x => x.LogTimeStamp.Ticks));
        }
        OnPropertyChanged(nameof(LogEntries));
        CurrentSortProperty = "DateTime";
        CurrentSortDirection = Ascending ? "Ascending" : "Descending";
        UpdateSortHeader();
    }

    [RelayCommand]
    private void SortByProperty(string propertyName)
    {
        var propertyInfo = typeof(LogEntry).GetProperty(propertyName);

        if (propertyInfo == null)
        {
            Console.WriteLine($"Property not found: {propertyName}");
            return;
        }

        if (Ascending)
        {
            LogEntries = new ObservableCollection<LogEntry>(LogEntries.OrderBy(x => propertyInfo.GetValue(x, null)));
        }
        else
        {
            LogEntries = new ObservableCollection<LogEntry>(LogEntries.OrderByDescending(x => propertyInfo.GetValue(x, null)));
        }

        OnPropertyChanged(nameof(LogEntries));
        CurrentSortProperty = propertyName;
        CurrentSortDirection = Ascending ? "Ascending" : "Descending";
        UpdateSortHeader();
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