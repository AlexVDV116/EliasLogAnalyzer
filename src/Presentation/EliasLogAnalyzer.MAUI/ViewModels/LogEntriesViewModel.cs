using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class LogEntriesViewModel : ObservableObject
{
    #region Fields

    private readonly ILogDataSharingService _logDataSharingService;

    #endregion

    #region Properties

    [ObservableProperty] private string _searchText;

    [ObservableProperty] private ObservableCollection<LogEntry> _filteredLogEntries;

    [ObservableProperty] private ObservableCollection<string> _selectedLogEntryDataLeft = [];
    [ObservableProperty] private ObservableCollection<string> _selectedLogEntryDataRight = [];

    [ObservableProperty] private IList<object> _selectedLogEntries = [];

    [ObservableProperty] private string _currentSortProperty;

    [ObservableProperty] private string _sortHeader;

    [ObservableProperty] private string _currentSortDirection;

    [ObservableProperty] private string _ascendingText = "✓ Ascending";

    [ObservableProperty] private string _descendingText = "Descending";

    [ObservableProperty] private string _sortDateTimeText = "DateTime";

    [ObservableProperty] private string _sortLogTypeText = "LogType";

    [ObservableProperty] private string _sortThreadText = "Thread/No";

    [ObservableProperty] private string _sortSourceLocationText = "Source Location";

    [ObservableProperty] private string _sortSourceText = "Source";

    [ObservableProperty] private string _sortCategoryText = "Category";

    [ObservableProperty] private string _sortEventIdText = "Event ID";

    [ObservableProperty] private string _sortUserText = "User";

    [ObservableProperty] private string _sortComputerText = "Computer";

    [ObservableProperty] private string _sortDescriptionText = "Computer";

    [ObservableProperty] private string _sortDateTimeHeaderText = "DateTime";

    [ObservableProperty] private string _sortLogTypeHeaderText = "LogType";

    [ObservableProperty] private string _sortThreadHeaderText = "Thread/No";

    [ObservableProperty] private string _sortSourceLocationHeaderText = "Source Location";

    [ObservableProperty] private string _sortSourceHeaderText = "Source";

    [ObservableProperty] private string _sortCategoryHeaderText = "Category";

    [ObservableProperty] private string _sortEventIdHeaderText = "Event ID";

    [ObservableProperty] private string _sortUserHeaderText = "User";

    [ObservableProperty] private string _sortComputerHeaderText = "Computer";

    [ObservableProperty] private string _sortDescriptionHeaderText = "Description";

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

    #endregion

    #region Constructor

    public LogEntriesViewModel(
        ILogDataSharingService logDataSharingService
    )
    {
        _logDataSharingService = logDataSharingService;
        AggregateLogEntries();
        SortByDateTime();
        RefreshFilter();
    }

    #endregion

    #region Commands

    [RelayCommand]
    private void SelectionChanged()
    {
        if (SelectedLogEntries.Count > 2)
        {
            SelectedLogEntries.RemoveAt(0);
        }

        UpdateSelectedEntryData();
    }

// Command to refresh filter whenever search text changes
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
                (entry.Description.Contains(lowerCaseSearchText, StringComparison.OrdinalIgnoreCase)) ||
                (entry.User.Contains(lowerCaseSearchText, StringComparison.OrdinalIgnoreCase)) ||
                (entry.Data.Contains(lowerCaseSearchText, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            FilteredLogEntries = new ObservableCollection<LogEntry>(filtered);
        }
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
    private void SetSortOrderAscending()
    {
        Ascending = true;
    }

    [RelayCommand]
    private void SetSortOrderDescending()
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

    #endregion

    #region Utility Methods

    partial void OnSearchTextChanged(string value)
    {
        RefreshFilter();
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
        SortDescriptionText = sortProperty == "Description" ? "✓ Description" : "Description";

        SortDateTimeHeaderText = "DateTime " + (sortProperty == "DateTime" ? (Ascending ? "▲" : "▼") : "");
        SortLogTypeHeaderText = "LogType " + (sortProperty == "LogType" ? (Ascending ? "▲" : "▼") : "");
        SortThreadHeaderText = "Thread/No " + (sortProperty == "ThreadNameOrNumber" ? (Ascending ? "▲" : "▼") : "");
        SortSourceLocationHeaderText =
            "Source Location " + (sortProperty == "SourceLocation" ? (Ascending ? "▲" : "▼") : "");
        SortSourceHeaderText = "Source " + (sortProperty == "Source" ? (Ascending ? "▲" : "▼") : "");
        SortCategoryHeaderText = "Category " + (sortProperty == "Category" ? (Ascending ? "▲" : "▼") : "");
        SortEventIdHeaderText = "Event ID " + (sortProperty == "EventId" ? (Ascending ? "▲" : "▼") : "");
        SortUserHeaderText = "User " + (sortProperty == "User" ? (Ascending ? "▲" : "▼") : "");
        SortComputerHeaderText = "Computer " + (sortProperty == "Computer" ? (Ascending ? "▲" : "▼") : "");
        SortDescriptionHeaderText = "Description " + (sortProperty == "Description" ? (Ascending ? "▲" : "▼") : "");
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

    private void UpdateSelectedEntryData()
    {
        if (SelectedLogEntries.Any())
        {
            SelectedLogEntryDataLeft.Clear();
            SelectedLogEntryDataRight.Clear();

            if (SelectedLogEntries[0] is LogEntry firstEntry)
            {
                SelectedLogEntryDataLeft.Add(firstEntry.Data);
            }

            if (SelectedLogEntries.Count > 1)
            {
                if (SelectedLogEntries[1] is LogEntry secondEntry)
                {
                    SelectedLogEntryDataRight.Add(secondEntry.Data);
                }
            }
        }

        #endregion
    }
}