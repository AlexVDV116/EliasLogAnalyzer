using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EliasLogAnalyzer.MAUI.Pages;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using System;

namespace EliasLogAnalyzer.MAUI.ViewModels
{
    public partial class SideMenuViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly Dictionary<string, Action> _pageSelectionActions;


        [ObservableProperty] private bool _isLogEntryPageSelected = true;
        [ObservableProperty] private bool _isLogEntryPageSelectable = false;
        [ObservableProperty] private bool _isTimelinePageSelected;
        [ObservableProperty] private bool _isTimelinePageSelectable = true;
        [ObservableProperty] private bool _isSummarizePageSelected;
        [ObservableProperty] private bool _isSummarizePageSelectable = true;
        [ObservableProperty] private bool _isDatabasePageSelected;
        [ObservableProperty] private bool _isDatabasePageSelectable = true;
        [ObservableProperty] private bool _isSettingsPageSelected;
        [ObservableProperty] private bool _isSettingsPageSelectable = true;

        public SideMenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _navigationService.OnNavigationChanged += OnNavigationChanged;

            _pageSelectionActions = new Dictionary<string, Action>
            {
                { nameof(LogEntriesPage), () => { IsLogEntryPageSelected = true; IsLogEntryPageSelectable = false; } },
                { nameof(TimelinePage), () => { IsTimelinePageSelected = true; IsTimelinePageSelectable = false; } },
                { nameof(SummarizePage), () => { IsSummarizePageSelected = true; IsSummarizePageSelectable = false; } },
                { nameof(DatabasePage), () => { IsDatabasePageSelected = true; IsDatabasePageSelectable = false; } },
                { nameof(SettingsPage), () => { IsSettingsPageSelected = true; IsSettingsPageSelectable = false; } }
            };
        }


        [RelayCommand]
        private void NavigateToPage(Type pageType) => _navigationService.NavigateToAsync(pageType);

        [RelayCommand]
        private void NavigateBack() => _navigationService.NavigateBackAsync();

        private void OnNavigationChanged(Type pageType)
        {
            ResetSelections();
            if (_pageSelectionActions.TryGetValue(pageType.Name, out var selectPageAction))
            {
                selectPageAction();
            }
        }


        private void ResetSelections()
        {
            IsLogEntryPageSelected = false;
            IsLogEntryPageSelectable = true;
            IsSettingsPageSelected = false;
            IsSettingsPageSelectable = true;
            IsTimelinePageSelected = false;
            IsTimelinePageSelectable = true;
            IsSummarizePageSelected = false;
            IsSummarizePageSelectable = true;
            IsDatabasePageSelected = false;
            IsDatabasePageSelectable = true;
        }
    }
}