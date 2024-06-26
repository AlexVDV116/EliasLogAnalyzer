﻿using EliasLogAnalyzer.MAUI.Services.Contracts;

namespace EliasLogAnalyzer.MAUI.Services;

public class DialogService : IDialogService
{
    public async Task ShowMessage(string title, string message)
    {
        if (Application.Current?.MainPage == null) return;
        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }

    public async Task<bool> ShowConfirmAsync(string title, string message, string accept, string cancel)
    {
        if (Application.Current?.MainPage == null)
            return false;

        return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
    }
}