using EliasLogAnalyzer.MAUI.Resources;

namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface ISettingsService
{
    Theme AppTheme { get; set; }
}