using CommunityToolkit.Maui;
using EliasLogAnalyzer.MAUI.Pages;
using EliasLogAnalyzer.MAUI.Services;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using EliasLogAnalyzer.MAUI.ViewModels;
using Microsoft.Extensions.Logging;

namespace EliasLogAnalyzer.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialSymbolsOutlined.ttf", "MaterialSymbols");
                });

            // UI components and their corresponding ViewModels
            builder.Services.AddSingleton<LogEntriesPage>();
            builder.Services.AddSingleton<LogEntriesViewModel>();
            builder.Services.AddTransient<TimelinePage>();
            builder.Services.AddTransient<TimelineViewModel>();
            builder.Services.AddTransient<SummarizePage>();
            builder.Services.AddTransient<SummarizeViewModel>();
            builder.Services.AddTransient<DatabasePage>();
            builder.Services.AddTransient<DatabaseViewModel>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<SettingsViewModel>();

            // Shared service across multiple components
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<ILogDataSharingService, LogDataSharingService>();
            builder.Services.AddSingleton<AppShellViewModel>();


            // Services used within a single ViewModel
            builder.Services.AddTransient<ILogFileLoaderService, LogFileLoaderService>();
            builder.Services.AddTransient<ILogFileParserService, LogFileParserService>();
            builder.Services.AddTransient<ILogEntryAnalysisService, LogEntryAnalysisService>();

#if WINDOWS
            // Hide CollectionView checkboxes when SelectionMode is Multiple
            Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("DisableMultiselectCheckbox",
            (handler, view) =>
            {
                handler.PlatformView.IsMultiSelectCheckBoxEnabled = false;
            });

#endif


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
