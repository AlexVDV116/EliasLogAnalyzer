using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class LogEntriesPage : ContentPage
{
    public LogEntriesPage(LogEntriesViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }
}