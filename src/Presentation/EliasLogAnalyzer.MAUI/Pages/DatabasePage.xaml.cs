using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class DatabasePage : ContentPage
{
    private readonly DatabaseViewModel _databaseViewModel;

    public DatabasePage(DatabaseViewModel databaseViewModel)
    {
        InitializeComponent();
        _databaseViewModel = databaseViewModel;
        BindingContext = databaseViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _databaseViewModel.CheckConnectionCommand.Execute(null);
    }
}