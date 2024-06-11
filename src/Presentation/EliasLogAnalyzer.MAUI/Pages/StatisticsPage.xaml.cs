using EliasLogAnalyzer.MAUI.ViewModels;
using Microsoft.Maui.Controls;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class StatisticsPage : ContentPage
{
    public StatisticsPage(StatisticsViewModel statisticsViewModel)
    {
        InitializeComponent();

        BindingContext = statisticsViewModel;
    }
}