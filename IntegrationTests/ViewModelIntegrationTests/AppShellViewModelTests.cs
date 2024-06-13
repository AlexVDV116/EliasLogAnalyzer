using EliasLogAnalyzer.MAUI.Services;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using EliasLogAnalyzer.MAUI.ViewModels;
using Xunit;

namespace IntegrationTests.ViewModelIntegrationTests;

public class AppShellViewModelTests
{
    private readonly AppShellViewModel _viewModel;

    public AppShellViewModelTests()
    {
        // Assuming SettingsService is your real implementation of ISettingsService
        ISettingsService settingsService = new SettingsService(); // Configure this to use a test configuration or environment
        _viewModel = new AppShellViewModel(settingsService);
    }

    [Fact]
    public void ToggleFlyoutWidth_Should_ToggleFlyoutWidthAndHeaderTextVisibility()
    {
        // Initial values
        Assert.Equal(80, _viewModel.FlyoutWidth);
        Assert.False(_viewModel.ToggleHeaderTextVisibility);

        // Act
        _viewModel.ToggleFlyoutWidthCommand.Execute(null);

        // Assert
        Assert.Equal(160, _viewModel.FlyoutWidth);
        Assert.True(_viewModel.ToggleHeaderTextVisibility);

        // Act again to toggle back
        _viewModel.ToggleFlyoutWidthCommand.Execute(null);

        // Assert reverted
        Assert.Equal(80, _viewModel.FlyoutWidth);
        Assert.False(_viewModel.ToggleHeaderTextVisibility);
    }
}