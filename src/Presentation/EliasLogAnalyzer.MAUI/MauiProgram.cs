﻿using CommunityToolkit.Maui;
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
            builder.Services.AddTransient<LogEntriesPage>();
            builder.Services.AddTransient<LogEntriesViewModel>();
            
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<ILogDataSharingService, LogDataSharingService>();
            builder.Services.AddSingleton<ILogFileLoaderService, LogFileLoaderService>();
            builder.Services.AddSingleton<ILogFileParserService, LogFileParserService>();
            builder.Services.AddSingleton<ILogEntryAnalysisService, LogEntryAnalysisService>();

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
