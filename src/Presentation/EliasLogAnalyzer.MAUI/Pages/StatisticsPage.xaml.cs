using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class StatisticsPage : ContentPage
{
    public StatisticsPage(StatisticsViewModel statisticsViewModel)
    {
        InitializeComponent();

        BindingContext = statisticsViewModel;
    }
}