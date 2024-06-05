using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class ReportPage : ContentPage
{
    public ReportPage(ReportViewModel reportViewModel)
    {
        InitializeComponent();

        BindingContext = reportViewModel;
    }
}