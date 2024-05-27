using EliasLogAnalyzer.MAUI.Pages;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EliasLogAnalyzer.MAUI.Services
{
    public class NavigationService : INavigationService
    {
        public event Action<Type>? OnNavigationChanged;

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NavigationService> _logger;

        private readonly Dictionary<string, string> _routes = new Dictionary<string, string>
        {
            { nameof(LogEntriesPage), "//LogEntriesPage" },
            { nameof(TimelinePage), "//TimeLinePage" },
            { nameof(SummarizePage), "//SummarizePage" },
            { nameof(DatabasePage), "//DatabasePage" },
            { nameof(SettingsPage), "//SettingsPage" }
        };

        public NavigationService(IServiceProvider serviceProvider, ILogger<NavigationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        public async Task NavigateToAsync(Type pageType)
        {
            try
            {
                if (_routes.TryGetValue(pageType.Name, out var route))
                {
                    await Shell.Current.GoToAsync(route);
                    OnNavigationChanged?.Invoke(pageType);
                }
                else
                {
                    _logger.LogWarning($"No route found for {pageType.Name}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception during navigation to {pageType.Name}");
            }
        }

        public async Task NavigateBackAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during navigation back");
            }
        }
    }
}
