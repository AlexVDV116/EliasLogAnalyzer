using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class LogEntriesPage
{

    public LogEntriesPage(LogEntriesViewModel logEntriesViewModel)
    {
        InitializeComponent();
        BindingContext = logEntriesViewModel;
    }
}