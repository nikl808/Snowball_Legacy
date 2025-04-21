using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Snowball_Legacy.Server.Contexts;
using Snowball_Legacy.Server.Controllers;
using Snowball_Legacy.Server.Models;

namespace Snowball_Legacy.Test;

public class GameFileControllerTests
{
    private readonly DataContext _context;
    private readonly GameFileController _controller;

    public GameFileControllerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new DataContext(options);

        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        var logger = new LoggerFactory().CreateLogger<GameFileController>();
        _controller = new GameFileController(_context, logger);
    }

    [Fact]
    public void CheckNoAdditionalFiles()
    {
        // Act
        var result = _controller.GetAdditionalFiles(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No additional files found for GameId: 1", notFoundResult.Value);
    }

    [Fact]
    public void GetAdditionalFiles()
    {
        // Arrange
        var fileData = new byte[] { 1, 2, 3, 4, 5 };
        _context.GameFile.Add(new GameFile
        {
            GameId = 1,
            File = fileData
        });
        _context.SaveChanges();

        // Act
        var result = _controller.GetAdditionalFiles(1);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/zip", fileResult.ContentType);
        Assert.Equal("files.zip", fileResult.FileDownloadName);
        Assert.Equal(fileData, fileResult.FileContents);
    }
}
