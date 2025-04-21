using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Snowball_Legacy.Server.Contexts;
using Snowball_Legacy.Server.Controllers;
using Snowball_Legacy.Server.Models;
using Snowball_Legacy.Server.Models.Dtos;
using Snowball_Legacy.Server.Models.ViewModels;

namespace Snowball_Legacy.Test;

public class GameControllerTests
{
    private readonly DataContext _context;
    private readonly GameController _controller;

    public GameControllerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new DataContext(options);

        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        var logger = new LoggerFactory().CreateLogger<GameController>();
        _controller = new GameController(_context, logger);
    }

    [Fact]
    public async Task GetListOfGames()
    {
        // Arrange
        _context.Game.AddRange(
            new Game { Id = 1, Name = "Game 1" },
            new Game { Id = 2, Name = "Game 2" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetGames();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<GameDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public async Task GetGameInfo()
    {
        // Act
        var result = await _controller.GetGameInfo(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Game with Id 1 not found", notFoundResult.Value);
    }

    [Fact]
    public async Task AddNewGame()
    {
        // Arrange
        var gameViewModel = new GameViewModel
        {
            Name = "New Game",
            Developer = "Developer",
            Genre = "Genre",
            ReleaseDate = "21.04.2025",
            Description = "Description",
            DiscNumber = 1,
            IsAdditionalFiles = 0
        };

        // Act
        var result = await _controller.AddGame(gameViewModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Game successfully uploaded.", okResult.Value);

        // Check that the game was added to the database
        var gameInDb = await _context.Game.FirstOrDefaultAsync(g => g.Name == "New Game");
        Assert.NotNull(gameInDb);
    }

    [Fact]
    public async Task UpdateGame()
    {
        // Arrange
        var gameViewModel = new GameViewModel
        {
            Id = 1,
            Name = "Updated Game",
            Developer = "Updated Developer",
            Genre = "Updated Genre",
            ReleaseDate = "21.04.2025",
            Description = "Updated Description",
            DiscNumber = 2,
            IsAdditionalFiles = 1
        };

        // Act
        var result = await _controller.UpdateGame(gameViewModel);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Game with Id 1 not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteGame()
    {
        // Arrange
        var game = new Game { Id = 1, Name = "Game 1" };
        _context.Game.Add(game);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteGame(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Game: 1 is removed", okResult.Value);

        // Check that the game was deleted from the database
        var gameInDb = await _context.Game.FindAsync(1);
        Assert.Null(gameInDb);
    }
}