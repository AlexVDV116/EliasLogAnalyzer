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

            // HttpClient
            builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("https://localhost:7028/") });

            // UI components and their corresponding ViewModels
            builder.Services.AddSingleton<LogEntriesPage>();
            builder.Services.AddSingleton<LogEntriesViewModel>();
            builder.Services.AddTransient<StatisticsPage>();
            builder.Services.AddTransient<StatisticsViewModel>();
            builder.Services.AddTransient<ReportPage>();
            builder.Services.AddTransient<ReportViewModel>();
            builder.Services.AddTransient<DatabasePage>();
            builder.Services.AddTransient<DatabaseViewModel>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<SettingsViewModel>();

            // Shared service across multiple components
            builder.Services.AddSingleton<AppShellViewModel>();
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<ILogDataSharingService, LogDataSharingService>();
            builder.Services.AddSingleton<IApiService, ApiService>();



            // Services used within a single ViewModel
            builder.Services.AddTransient<ILogFileLoaderService, LogFileLoaderService>();
            builder.Services.AddTransient<ILogFileParserService, LogFileParserService>();
            builder.Services.AddTransient<ILogEntryAnalysisService, LogEntryAnalysisService>();
            builder.Services.AddTransient<IHtmlGeneratorService, HtmlGeneratorService>();
            builder.Services.AddTransient<IDialogService, DialogService>();
            builder.Services.AddTransient<IHashService, HashService>();

            // Hide CollectionView checkboxes when SelectionMode is Multiple
#if WINDOWS
            Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("DisableMultiselectCheckbox",
            (handler, view) =>
            {
                handler.PlatformView.IsMultiSelectCheckBoxEnabled = false;
            });
#elif DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
