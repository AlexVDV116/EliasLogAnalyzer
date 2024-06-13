using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI
{
    public partial class App
    {

        public App(AppShellViewModel appShellViewModel, LogEntriesViewModel logEntriesViewModel)
        {

            InitializeComponent();
            MainPage = new AppShell(appShellViewModel, logEntriesViewModel);
            // Setting the theme currently bugs out the flyout menu https://github.com/dotnet/maui/issues/22672
            //SetTheme();
        }

        private static void SetTheme()
        {
            if (Application.Current != null)
                Application.Current.UserAppTheme = AppTheme.Light;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

            const int newWidth = 1920;
            const int newHeight = 1080;

            window.Width = newWidth;
            window.Height = newHeight;

            window.MinimumHeight = newHeight;
            window.MinimumWidth = newWidth;

            return window;
        }
    }
}
