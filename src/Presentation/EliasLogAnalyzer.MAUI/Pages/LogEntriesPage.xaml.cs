using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class LogEntriesPage : ContentPage
{

    public LogEntriesPage(LogEntriesViewModel logEntriesViewModel)
    {
        InitializeComponent();
        BindingContext = logEntriesViewModel;
    }
}