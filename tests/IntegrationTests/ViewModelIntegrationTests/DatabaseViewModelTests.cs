using EliasLogAnalyzer.MAUI.Services;
using EliasLogAnalyzer.MAUI.ViewModels;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IntegrationTests.ViewModelIntegrationTests;

public class DatabaseViewModelTests
{
    private readonly DatabaseViewModel _viewModel;

    public DatabaseViewModelTests()
    {
        var httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7028/") };
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();  // or builder.AddConsole() depending on what is available
        });
        var apiLogger = loggerFactory.CreateLogger<ApiService>();

        var apiService = new ApiService(httpClient, apiLogger);
        _viewModel = new DatabaseViewModel(apiService);
    }


    [Fact]
    public async Task CheckConnectionCommand_Should_UpdateViewModel()
    {
        // Act
        await _viewModel.CheckConnectionCommand.ExecuteAsync(null);

        // Assert
        if (_viewModel.IsConnected)
        {
            Assert.True(_viewModel.ConnectedIconVisible);
            Assert.False(_viewModel.NotConnectedIconVisible);
            Assert.Equal("Connected", _viewModel.ConnectionStatus);
        }
        else
        {
            Assert.False(_viewModel.ConnectedIconVisible);
            Assert.True(_viewModel.NotConnectedIconVisible);
            Assert.NotEqual("Connected", _viewModel.ConnectionStatus);
        }
    }
}