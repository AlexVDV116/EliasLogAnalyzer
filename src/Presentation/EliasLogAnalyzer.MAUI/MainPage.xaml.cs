using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI
{
    public partial class MainPage
    {

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }

}
