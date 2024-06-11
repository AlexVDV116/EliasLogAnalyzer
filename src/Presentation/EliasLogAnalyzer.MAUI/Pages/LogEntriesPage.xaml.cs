using EliasLogAnalyzer.MAUI.ViewModels;
using Microsoft.Maui.Controls;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class LogEntriesPage : ContentPage
{

    public LogEntriesPage(LogEntriesViewModel logEntriesViewModel)
    {
        InitializeComponent();
        BindingContext = logEntriesViewModel;
    }
}