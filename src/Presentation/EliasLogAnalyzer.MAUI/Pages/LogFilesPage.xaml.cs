using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class LogFilesPage
{
    public LogFilesPage(LogFilesViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;

    }
}