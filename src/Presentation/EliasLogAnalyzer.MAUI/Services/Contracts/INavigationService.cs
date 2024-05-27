namespace EliasLogAnalyzer.MAUI.Services.Contracts
{
    public interface INavigationService
    {
        event Action<Type>? OnNavigationChanged;
        Task NavigateToAsync(Type pageType);
        Task NavigateBackAsync();
    }
}
