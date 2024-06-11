using EliasLogAnalyzer.MAUI.Services.Contracts;
using EliasLogAnalyzer.MAUI.ViewModels;
using Moq;
using Xunit;

namespace UnitTests.IntegrationTests;

public class AppShellViewModelTests
{
    private readonly Mock<ISettingsService> _settingsServiceMock;
    private readonly AppShellViewModel _viewModel;

    public AppShellViewModelTests()
    {
        _settingsServiceMock = new Mock<ISettingsService>();
        _viewModel = new AppShellViewModel(_settingsServiceMock.Object);
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