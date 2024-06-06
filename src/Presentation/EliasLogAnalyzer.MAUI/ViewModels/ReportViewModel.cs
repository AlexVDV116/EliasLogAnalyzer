using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EliasLogAnalyzer.MAUI.ViewModels;

public partial class ReportViewModel : ObservableObject
{

    [ObservableProperty] private string developerName = string.Empty;
    [ObservableProperty] private string workstationName = string.Empty;
    [ObservableProperty] private DateTime reportDateTime = DateTime.Now;
    [ObservableProperty] private string situation = string.Empty;
    [ObservableProperty] private string observation = string.Empty;
    [ObservableProperty] private string expectation = string.Empty;
    [ObservableProperty] private string tag = string.Empty;
    [ObservableProperty] private string build = string.Empty;
    [ObservableProperty] private string severity = string.Empty;
    [ObservableProperty] private string analysis = string.Empty;
    [ObservableProperty] private string possibleSolutions = string.Empty;
    [ObservableProperty] private string whatToTest = string.Empty;
    [ObservableProperty] private string effort = string.Empty;
    [ObservableProperty] private string risk = string.Empty;
    [ObservableProperty] private string workaround = string.Empty;
    [ObservableProperty] private string recommendation = string.Empty;

    public List<string> Severities { get; } = ["Critical", "High", "Medium", "Low"]; // Severity levels

    public ReportViewModel()
    {
    }

    [RelayCommand]
    private void Submit()
    {
        // Logic to handle the submission of the form, e.g., validation, saving to a database, or sending over a network
        Console.WriteLine($"Report submitted by {DeveloperName} regarding {Situation}");
        // Add actual logic to handle data persistence or API interaction
    }
}