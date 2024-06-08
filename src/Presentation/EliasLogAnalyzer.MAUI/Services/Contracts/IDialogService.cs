namespace EliasLogAnalyzer.MAUI.Services.Contracts;

public interface IDialogService
{
    Task ShowMessage(string title, string message);
    Task<bool> ShowConfirmAsync(string title, string message, string accept, string cancel);
}
