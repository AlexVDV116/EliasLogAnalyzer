using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI
{
    public partial class AppShell
    {
        private readonly LogEntriesViewModel _logEntriesViewModel;
        private readonly AppShellViewModel _appShellViewModel;

        public AppShell(AppShellViewModel appShellViewModel, LogEntriesViewModel logEntriesViewModel)
        {
            InitializeComponent();
            _appShellViewModel = appShellViewModel;
            _logEntriesViewModel = logEntriesViewModel;
            BindingContext = _appShellViewModel;
        }

        // Reset binding context after page change due to https://github.com/dotnet/maui/issues/13848
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Set BindingContext for specific MenuBarItems
            FileMenuBarItem.BindingContext = _logEntriesViewModel;
            FilterMenuBarItem.BindingContext = _logEntriesViewModel;
            ViewMenuBarItem.BindingContext = _appShellViewModel;
        }
    }
}
