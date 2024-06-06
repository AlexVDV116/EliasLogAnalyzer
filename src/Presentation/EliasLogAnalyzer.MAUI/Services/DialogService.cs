using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.Services
{
    internal class DialogService : IDialogService
    {
        public async Task<bool> ShowConfirmAsync(string title, string message, string accept, string cancel)
        {
            if (Application.Current?.MainPage == null)
                return false;

            return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }
    }
}
