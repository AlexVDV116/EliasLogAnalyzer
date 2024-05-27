using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class DatabasePage : ContentPage
{
    public DatabasePage(DatabaseViewModel databaseViewModel)
    {
        InitializeComponent();

        BindingContext = databaseViewModel;
    }
}