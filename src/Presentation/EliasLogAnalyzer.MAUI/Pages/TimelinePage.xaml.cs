using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class TimelinePage : ContentPage
{
    public TimelinePage(TimelineViewModel timelineViewModel)
    {
        InitializeComponent();

        BindingContext = timelineViewModel;
    }
}