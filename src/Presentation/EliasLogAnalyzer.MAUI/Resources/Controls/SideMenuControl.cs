using EliasLogAnalyzer.MAUI.Pages;
using EliasLogAnalyzer.MAUI.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace EliasLogAnalyzer.MAUI.Resources.Controls;

public class SideMenuControl : ContentView
{

    public SideMenuControl()
    {
        // Set BindingContext of the control to its viewModel
        BindingContext = App.Current?.Handler?.MauiContext?.Services.GetService<SideMenuViewModel>();

        var grid = new Grid
        {
            WidthRequest = 80,
            Background = new SolidColorBrush(Color.FromRgb(211, 211, 211)), // LightGray
            Padding = new Thickness(0, 100, 0, 0)

        };

        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        AddMenuButton(grid, 0, "list.png", typeof(LogEntriesPage), "IsLogEntryPageSelected", "IsLogEntryPageSelectable");
        AddMenuButton(grid, 1, "timeline.png", typeof(TimelinePage), "IsTimelinePageSelected", "IsTimelinePageSelectable");
        AddMenuButton(grid, 2, "summarize.png", typeof(SummarizePage), "IsSummarizePageSelected", "IsSummarizePageSelectable");
        AddMenuButton(grid, 3, "database.png", typeof(DatabasePage), "IsDatabasePageSelected", "IsDatabasePageSelectable");
        AddMenuButton(grid, 4, "settings.png", typeof(SettingsPage), "IsSettingsPageSelected", "IsSettingsPageSelectable");

        Content = grid;
    }

    private void AddMenuButton(Grid grid, int rowIndex, string imageFileName, Type pageType, string isVisibleBinding, string isEnabledBinding)
    {
        var buttonGrid = new Grid();
        Grid.SetRow(buttonGrid, rowIndex);

        var border = new Border
        {
            Background = Colors.White,
            WidthRequest = 80,
            HeightRequest = 60,
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10, 0, 10, 0) },
            Margin = new Thickness(10, 0, 0, rowIndex == 4 ? 20 : 0),
            VerticalOptions = rowIndex == 4 ? LayoutOptions.End : LayoutOptions.Center
        };

        // IsVisible Binding
        var isVisibleBindingSetup = new Binding(isVisibleBinding);
        border.SetBinding(IsVisibleProperty, isVisibleBindingSetup);

        var buttonImage = new ImageButton
        {
            Source = ImageSource.FromFile($"Resources/Images/{imageFileName}"),
            HeightRequest = 40,
            WidthRequest = 40,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = rowIndex == 4 ? LayoutOptions.End : LayoutOptions.Center,
            Margin = rowIndex == 4 ? new Thickness(0, 0, 0, 30) : new Thickness(0)
        };

        // Prevent the opacity to be set when the button isEnabled
        VisualStateManager.SetVisualStateGroups(buttonImage, new VisualStateGroupList
        {
            new VisualStateGroup
            {
                Name = "CommonStates",
                States =
                {
                    new VisualState { Name = "Normal", Setters = { new Setter { Property = ImageButton.OpacityProperty, Value = 1 } } },
                    new VisualState { Name = "Disabled", Setters = { new Setter { Property = ImageButton.OpacityProperty, Value = 1 } } }
                }
            }
        });

        // Set bindings
        buttonImage.SetBinding(Button.CommandProperty, new Binding("NavigateToPageCommand"));
        buttonImage.CommandParameter = pageType;
        border.SetBinding(IsVisibleProperty, new Binding(isVisibleBinding));

        buttonGrid.Children.Add(border);
        buttonGrid.Children.Add(buttonImage);

        grid.Children.Add(buttonGrid);
    }
}

