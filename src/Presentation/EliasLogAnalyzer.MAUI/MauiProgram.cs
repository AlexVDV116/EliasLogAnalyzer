using EliasLogAnalyzer.MAUI.Services;
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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<ISettingsService, SettingsService>();
            builder.Services.AddTransient<ILogFileLoaderService, LogFileLoaderService>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
