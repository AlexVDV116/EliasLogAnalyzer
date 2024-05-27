using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class SummarizePage : ContentPage
{
    public SummarizePage(SummarizeViewModel summarizeViewModel)
    {
        InitializeComponent();

        BindingContext = summarizeViewModel;
    }
}