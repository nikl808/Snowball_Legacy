using Microsoft.AspNetCore.Mvc;
using Moq;
using Snowball_Legacy.Application.DTOs;
using Snowball_Legacy.Application.Interfaces;
using Snowball_Legacy.Server.Controllers;
using Xunit;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Snowball_Legacy.Test;

public class GameControllerTests
{
    private readonly Mock<IGenericService<GameDto>> _serviceMock;
    private readonly Mock<ILogger<GameController>> _loggerMock;
    private readonly GameController _controller;
    private readonly CancellationToken _token = CancellationToken.None;

    public GameControllerTests()
    {
        _serviceMock = new Mock<IGenericService<GameDto>>();
        _loggerMock = new Mock<ILogger<GameController>>();
        _controller = new GameController(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsListOfGames()
    {
        // Arrange
        var games = new List<GameDto> { new() { Id = 1, Name = "Game 1" }, new() { Id = 2, Name = "Game 2" } };
        _serviceMock.Setup(s => s.GetAllAsync(_token)).ReturnsAsync(games);

        // Act
        var result = await _controller.GetAll(_token);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, g => g.Name == "Game 1");
    }

    [Fact]
    public async Task GetById_ReturnsGame_WhenFound()
    {
        // Arrange
        var game = new GameDto { Id = 1, Name = "Game 1" };
        _serviceMock.Setup(s => s.GetByIdAsync(1, _token)).ReturnsAsync(game);

        // Act
        var result = await _controller.GetById(1, _token);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGame = Assert.IsType<GameDto>(okResult.Value);
        Assert.Equal(1, returnedGame.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync(1, _token)).ReturnsAsync((GameDto?)null);

        // Act
        var result = await _controller.GetById(1, _token);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task AddGame_ReturnsOkWithGame()
    {
        // Arrange
        var gameDto = new GameDto { Id = 1, Name = "New Game" };
        _serviceMock.Setup(s => s.AddAsync(gameDto, _token)).ReturnsAsync(gameDto);

        // Act
        var result = await _controller.AddGame(gameDto, _token);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGame = Assert.IsType<GameDto>(okResult.Value);
        Assert.Equal("New Game", returnedGame.Name);
    }

    [Fact]
    public async Task UpdateGame_ReturnsNoContent()
    {
        // Arrange
        var gameDto = new GameDto { Id = 1, Name = "Updated Game" };
        _serviceMock.Setup(s => s.UpdateAsync(gameDto, _token)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateGame(gameDto, _token);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteAsync(1, _token)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1, _token);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}