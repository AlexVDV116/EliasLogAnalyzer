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
    private string searchText;

    [ObservableProperty]
    private ObservableCollection<LogEntry> filteredLogEntries;

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

    [ObservableProperty]
    private string _sortDateTimeText = "DateTime";

    [ObservableProperty]
    private string _sortLogTypeText = "LogType";

    [ObservableProperty]
    private string _sortThreadText = "Thread/No";

    [ObservableProperty]
    private string _sortSourceLocationText = "Source Location";

    [ObservableProperty]
    private string _sortSourceText = "Source";

    [ObservableProperty]
    private string _sortCategoryText = "Category";

    [ObservableProperty]
    private string _sortEventIdText = "Event ID";

    [ObservableProperty]
    private string _sortUserText = "User";

    [ObservableProperty]
    private string _sortComputerText = "Computer";

    [ObservableProperty]
    private string _sortDateTimeHeaderText = "DateTime";

    [ObservableProperty]
    private string _sortLogTypeHeaderText = "LogType";

    [ObservableProperty]
    private string _sortThreadHeaderText = "Thread/No";

    [ObservableProperty]
    private string _sortSourceLocationHeaderText = "Source Location";

    [ObservableProperty]
    private string _sortSourceHeaderText = "Source";

    [ObservableProperty]
    private string _sortCategoryHeaderText = "Category";

    [ObservableProperty]
    private string _sortEventIdHeaderText = "Event ID";

    [ObservableProperty]
    private string _sortUserHeaderText = "User";

    [ObservableProperty]
    private string _sortComputerHeaderText = "Computer";

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
        RefreshFilter();
    }

    partial void OnSearchTextChanged(string value)
    {
        RefreshFilter();
    }

    // Command to refresh filter whenever search text changes
    // 
    [RelayCommand]
    private void RefreshFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredLogEntries = new ObservableCollection<LogEntry>(LogEntries);
        }
        else
        {
            var lowerCaseSearchText = SearchText.ToLower();
            var filtered = LogEntries.Where(entry =>
                (entry.Description?.ToLower().Contains(lowerCaseSearchText) ?? false) ||
                (entry.User?.ToLower().Contains(lowerCaseSearchText) ?? false) ||
                (entry.Data?.ToLower().Contains(lowerCaseSearchText) ?? false) 
            );

            FilteredLogEntries = new ObservableCollection<LogEntry>(filtered);
        }
    }

    // Updates the MenuBarItems text and CollectionView Header text to display current sort direction and property
    private void UpdateSortTexts(string sortProperty)
    {
        SortDateTimeText = sortProperty == "DateTime" ? "✓ DateTime" : "DateTime";
        SortLogTypeText = sortProperty == "LogType" ? "✓ LogType" : "LogType";
        SortThreadText = sortProperty == "ThreadNameOrNumber" ? "✓ Thread/No" : "Thread/No";
        SortSourceLocationText = sortProperty == "SourceLocation" ? "✓ Source Location" : "Source Location";
        SortSourceText = sortProperty == "Source" ? "✓ Source" : "Source";
        SortCategoryText = sortProperty == "Category" ? "✓ Category" : "Category";
        SortEventIdText = sortProperty == "EventId" ? "✓ Event ID" : "Event ID";
        SortUserText = sortProperty == "User" ? "✓ User" : "User";
        SortComputerText = sortProperty == "Computer" ? "✓ Computer" : "Computer";

        SortDateTimeHeaderText = "DateTime " + (sortProperty == "DateTime" ? (Ascending ? "▲" : "▼") : "");
        SortLogTypeHeaderText = "LogType " + (sortProperty == "LogType" ? (Ascending ? "▲" : "▼") : "");
        SortThreadHeaderText = "Thread/No " + (sortProperty == "ThreadNameOrNumber" ? (Ascending ? "▲" : "▼") : "");
        SortSourceLocationHeaderText = "Source Location " + (sortProperty == "SourceLocation" ? (Ascending ? "▲" : "▼") : "");
        SortSourceHeaderText = "Source " + (sortProperty == "Source" ? (Ascending ? "▲" : "▼") : "");
        SortCategoryHeaderText = "Category " + (sortProperty == "Category" ? (Ascending ? "▲" : "▼") : "");
        SortEventIdHeaderText = "Event ID " + (sortProperty == "EventId" ? (Ascending ? "▲" : "▼") : "");
        SortUserHeaderText = "User " + (sortProperty == "User" ? (Ascending ? "▲" : "▼") : "");
        SortComputerHeaderText = "Computer " + (sortProperty == "Computer" ? (Ascending ? "▲" : "▼") : "");
    }

    [RelayCommand]
    private void HeaderTapped(string propertyName)
    {
        if (CurrentSortProperty == propertyName)
        {
            // Toggle the sort direction if the same header is tapped again
            Ascending = !Ascending;
        }
        else
        {
            // Change the sort property and default to ascending
            CurrentSortProperty = propertyName;
            Ascending = true;
        }

        if (CurrentSortProperty == "DateTime")
        {
            SortByDateTime();
        }
        else
        {
            SortByProperty(propertyName);
        }
        UpdateSortTexts(propertyName);
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
        RefreshFilter();
        OnPropertyChanged(nameof(LogEntries));
        CurrentSortProperty = "DateTime";
        CurrentSortDirection = Ascending ? "Ascending" : "Descending";
        UpdateSortTexts("DateTime");
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

        IEnumerable<LogEntry> sorted;
        if (Ascending)
        {
            sorted = LogEntries.OrderBy(x => propertyInfo.GetValue(x, null));
        }
        else
        {
            sorted = LogEntries.OrderByDescending(x => propertyInfo.GetValue(x, null));
        }

        LogEntries = new ObservableCollection<LogEntry>(sorted);
        RefreshFilter();
        OnPropertyChanged(nameof(LogEntries));
        UpdateSortTexts(propertyName);
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
        RefreshFilter();
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