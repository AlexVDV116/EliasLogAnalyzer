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
                });

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<LogFilesPage>();
            builder.Services.AddTransient<LogFilesViewModel>();
            
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<ILogDataSharingService, LogDataSharingService>();
            builder.Services.AddSingleton<ILogFileLoaderService, LogFileLoaderService>();
            builder.Services.AddSingleton<ILogFileParserService, LogFileParserService>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
