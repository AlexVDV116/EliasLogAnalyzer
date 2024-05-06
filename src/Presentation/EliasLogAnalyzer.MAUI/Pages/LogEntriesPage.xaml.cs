using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EliasLogAnalyzer.Domain.Entities;
using EliasLogAnalyzer.MAUI.ViewModels;

namespace EliasLogAnalyzer.MAUI.Pages;

public partial class LogEntriesPage : ContentPage
{
    public LogEntriesPage(LogEntriesViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
        
        Shell.SetNavBarIsVisible(this, true);
    }
}