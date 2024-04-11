﻿using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI
{
    public partial class MainPage : ContentPage
    {

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //await viewModel.Initialise();
        }
    }

}
