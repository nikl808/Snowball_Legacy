using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Snowball_Legacy.Server.Contexts;
using Snowball_Legacy.Server.Controllers;
using Snowball_Legacy.Server.Models;
using System.IO.Compression;

namespace Snowball_Legacy.Test;

public class GamePictureControllerTests
{
    private readonly DataContext _context;
    private readonly GamePictureController _controller;

    public GamePictureControllerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new DataContext(options);

        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        var logger = new LoggerFactory().CreateLogger<GamePictureController>();
        _controller = new GamePictureController(_context, logger);
    }

    [Fact]
    public void CheckNoTitlePicture()
    {
        // Act
        var result = _controller.GetTitlePicture(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFound<string>>(result);
        Assert.Equal("No title picture found for GameInfoId: 1", notFoundResult.Value);
    }

    [Fact]
    public void GetTitlePicture()
    {
        // Arrange
        var pictureData = new byte[] { 1, 2, 3 };
        _context.GameTitlePicture.Add(new GameTitlePicture
        {
            GameInfoId = 1,
            Picture = pictureData
        });
        _context.SaveChanges();

        // Act
        var result = _controller.GetTitlePicture(1);

        // Assert
        var fileResult = Assert.IsType<FileContentHttpResult>(result);
        Assert.Equal("image/jpeg", fileResult.ContentType);
        Assert.Equal(pictureData, fileResult.FileContents);
    }

    [Fact]
    public void CheckNoScreenshots()
    {
        // Act
        var result = _controller.GetScreenshots(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFound<string>>(result);
        Assert.Equal("No screenshots found for GameInfoId: 1", notFoundResult.Value);
    }

    [Fact]
    public void GetScreenshots()
    {
        // Arrange
        var screenshotData1 = new byte[] { 1, 2, 3 };
        var screenshotData2 = new byte[] { 4, 5, 6 };

        _context.GameScreenshot.AddRange(
            new GameScreenshots { GameInfoId = 1, Picture = screenshotData1 },
            new GameScreenshots { GameInfoId = 1, Picture = screenshotData2 }
        );
        _context.SaveChanges();

        // Act
        var result = _controller.GetScreenshots(1);

        // Assert
        var fileResult = Assert.IsType<FileContentHttpResult>(result);

        Assert.Equal("application/zip", fileResult.ContentType);

        using var memoryStream = new MemoryStream(fileResult.FileContents.ToArray());
        using var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

        Assert.Equal(2, zipArchive.Entries.Count);

        var entry1 = zipArchive.GetEntry("screenshot_1.jpg");
        Assert.NotNull(entry1);

        var entry2 = zipArchive.GetEntry("screenshot_2.jpg");
        Assert.NotNull(entry2);
    }
}
