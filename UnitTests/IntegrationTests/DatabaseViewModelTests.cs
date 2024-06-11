using EliasLogAnalyzer.BusinessLogic.Entities;
using EliasLogAnalyzer.MAUI.Services.Contracts;
using EliasLogAnalyzer.MAUI.ViewModels;
using Moq;
using Xunit;

namespace UnitTests.IntegrationTests;

// Purpose: This file is used to test the DatabaseViewModel class in the EliasLogAnalyzer.MAUI project. The DatabaseViewModel
// class is used to check the connection to the EliasLogAnalyzer database. The tests in this file verify that the CheckConnectionCommand
// method in the DatabaseViewModel class correctly checks the connection to the database.

public class DatabaseViewModelTests
{
    private readonly Mock<IApiService> _apiServiceMock;
    private readonly DatabaseViewModel _viewModel;

    public DatabaseViewModelTests()
    {
        _apiServiceMock = new Mock<IApiService>();
        _viewModel = new DatabaseViewModel(_apiServiceMock.Object);
    }

    [Fact]
    public async Task CheckConnectionCommand_Should_UpdateViewModel_When_ConnectionIsSuccessful()
    {
        // Arrange
        var apiResult = ApiResult.Ok();
        _apiServiceMock.Setup(service => service.CheckDatabaseConnectionAsync()).ReturnsAsync(apiResult);

        // Act
        await _viewModel.CheckConnectionCommand.ExecuteAsync(null);

        // Assert
        Assert.True(_viewModel.IsConnected);
        Assert.True(_viewModel.ConnectedIconVisible);
        Assert.False(_viewModel.NotConnectedIconVisible);
        Assert.Equal("Connected", _viewModel.ConnectionStatus);
    }

    [Fact]
    public async Task CheckConnectionCommand_Should_UpdateViewModel_When_ConnectionFails()
    {
        // Arrange
        var apiResult = ApiResult.Fail("Failed to connect to the database.");
        _apiServiceMock.Setup(service => service.CheckDatabaseConnectionAsync()).ReturnsAsync(apiResult);

        // Act
        await _viewModel.CheckConnectionCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_viewModel.IsConnected);
        Assert.False(_viewModel.ConnectedIconVisible);
        Assert.True(_viewModel.NotConnectedIconVisible);
        Assert.Equal("Failed to connect to the database.", _viewModel.ConnectionStatus);
    }
}