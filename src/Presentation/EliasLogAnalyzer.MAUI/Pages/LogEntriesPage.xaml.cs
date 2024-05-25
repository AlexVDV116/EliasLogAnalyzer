using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class LogEntriesPage : ContentPage
{
    public LogEntriesPage(LogEntriesViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }
}