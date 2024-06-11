using EliasLogAnalyzer.MAUI.ViewModels;
using Microsoft.Maui.Controls;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class ReportPage : ContentPage
{
    public ReportPage(ReportViewModel reportViewModel)
    {
        InitializeComponent();

        BindingContext = reportViewModel;
    }
}