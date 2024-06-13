using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class ReportPage
{

    private readonly ReportViewModel _reportViewModel;
    public ReportPage(ReportViewModel reportViewModel)
    {
        InitializeComponent();
        _reportViewModel = reportViewModel;
        BindingContext = reportViewModel;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _reportViewModel.CheckConnectionCommand.Execute(null);
    }
}