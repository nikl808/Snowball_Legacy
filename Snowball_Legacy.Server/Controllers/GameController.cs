using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Snowball_Legacy.Server.Contexts;
using Snowball_Legacy.Server.Models;
using Snowball_Legacy.Server.Models.Dtos;
using Snowball_Legacy.Server.Models.ViewModels;

namespace Snowball_Legacy.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController(
    DataContext context,
    ILogger<GameController> logger) : ControllerBase
{
    /// <summary>
    /// Getting a list of games
    /// </summary>
    /// <returns>List of GameDto</returns>
    [HttpGet("list", Name = "GetGames")]
    public async Task<ActionResult<IList<GameDto>>> GetGames()
    {
        try
        {
            var games = await context.Game
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    Name = g.Name
                }).ToListAsync();
            return games.Any() ? Ok(games) : NotFound("No games found.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while retrieving the game list.");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Getting the game
    /// </summary>
    /// <param name="gameId">Game Id</param>
    /// <returns>GameInfoDto</returns>
    [HttpGet("info/{gameId}", Name = "GetGameInfo")]
    public async Task<ActionResult<GameInfoDto>> GetGameInfo(int gameId)
    {
        try
        {
            var game = await context.Game.Include(i => i.GameInfo).FirstOrDefaultAsync(g => g.Id == gameId);
            if (game?.GameInfo is null)
                return NotFound($"Game with Id {gameId} not found");

            var info = new GameInfoDto()
            {
                Id = game.GameInfo.Id,
                Name = game.Name,
                Developer = game.GameInfo.Developer,
                Genre = game.GameInfo.Genre,
                ReleaseDate = game.GameInfo.ReleaseDate,
                IsAdditionalFiles = game.GameInfo.IsAdditionalFiles,
                DiscNumber = game.GameInfo.DiskNumber,
                Description = game.GameInfo.Description
            };
            return Ok(info);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error when receiving game info for Id {gameId}");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Add game
    /// </summary>
    /// <param name="vm">GameViewModel</param>
    /// <returns>Ok result or error</returns>
    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> AddGame([FromForm] GameViewModel vm)
    {
        try
        {
            // Create a game entity
            var game = new Game { Name = vm.Name };
            var gameInfo = ProccessGameInfo(game, vm);
            game.GameInfo = gameInfo;

            // Process title picture
            if (vm.TitlePicture?.Any() is true)
            {
                var titlePicture = await ProcessTitlePictureAsync(gameInfo, vm.TitlePicture[0]);
                await context.GameTitlePicture.AddAsync(titlePicture);
            }

            // Process screenshots
            if (vm.Screenshots?.Any() is true)
            {
                var screenshots = await ProcessScreenshotsAsync(gameInfo, vm.Screenshots);
                gameInfo.ScreenShoots = screenshots;
                await context.GameScreenshot.AddRangeAsync(screenshots);
            }

            // Add game and game info to the context
            await context.Game.AddAsync(game);
            await context.GameInfo.AddAsync(gameInfo);

            //Process additional files
            if (vm.AdditionalFiles?.Any() is true)
            {
                var gameFiles = await ProcessAdditionalFiles(game, vm.AdditionalFiles);
                await context.GameFile.AddRangeAsync(gameFiles);
            }

            // Save changes to the db
            await context.SaveChangesAsync();
            return Ok("Game successfully uploaded.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Game additional error");
            return StatusCode(500, "Internal server error");
        }

    }

    /// <summary>
    /// Update game
    /// </summary>
    /// <param name="vm">GameViewModel</param>
    /// <returns>Ok result or error</returns>
    [HttpPost("update", Name = "UpdateGame")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> UpdateGame([FromForm] GameViewModel vm)
    {
        try
        {
            //Get game and game info
            var game = await context.Game.Include(g => g.GameInfo).FirstOrDefaultAsync(g => g.Id == vm.Id);
            if (game is null || game.GameInfo is null) return NotFound($"Game with Id {vm.Id} not found.");

            //Update main game and game info
            game.Name = vm.Name;
            game.GameInfo = UpdateGameInfo(game.GameInfo, vm);

            //Update title picture
            if (vm.TitlePicture?.Any() is true)
                await UpdateTitlePictureAsync(game.GameInfo, vm.TitlePicture[0]);

            if (vm.Screenshots is not null)
                await UpdateScreenshotsAsync(game.GameInfo, vm.Screenshots);

            if (vm.AdditionalFiles is not null)
                await UpdateAdditionalFilesAsync(game, vm.AdditionalFiles);

            await context.SaveChangesAsync();
            return Ok("Game successfully updated");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while updating the game with ID {vm.Id}.");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete game
    /// </summary>
    /// <param name="gameId">GameId</param>
    /// <returns>Ok result or error</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteGame([FromHeader] int gameId)
    {
        var game = context.Game.Where(g => g.Id == gameId).FirstOrDefault();
        if (game is not null)
        {
            context.Game.Remove(game);
            await context.SaveChangesAsync();
            return Ok($"Game: {gameId} is removed");
        }
        logger.LogError(gameId, "Game is not found");
        return StatusCode(500, "Internal server error");
    }

    private async Task<byte[]> FileToByteArrayAsync(IFormFile file)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("File is null or empty.", nameof(file));
        
        using var memoryStream = new MemoryStream((int)file.Length);
        await file.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    private GameInfo ProccessGameInfo(Game game, GameViewModel vm)
    {
        return new GameInfo
        {
            Game = game,
            Developer = vm.Developer,
            IsAdditionalFiles = Convert.ToBoolean(vm.IsAdditionalFiles),
            Description = vm.Description,
            Genre = vm.Genre,
            DiskNumber = vm.DiscNumber,
            ReleaseDate = DateOnly.Parse(DateTime.ParseExact(vm.ReleaseDate, "dd.mm.yyyy", null).ToShortDateString())
        };
    }

    private async Task<GameTitlePicture> ProcessTitlePictureAsync(GameInfo gameInfo, IFormFile titlePictureFile)
    {
        var titlePicture = new GameTitlePicture
        {
            GameInfo = gameInfo,
            Picture = await FileToByteArrayAsync(titlePictureFile)
        };

        gameInfo.TitlePicture = titlePicture;
        return titlePicture;
    }

    private async Task<List<GameScreenshots>> ProcessScreenshotsAsync(GameInfo gameInfo, List<IFormFile> screenshotFiles)
    {
        var screenshots = new List<GameScreenshots>();
        foreach (var screenshoot in screenshotFiles)
        {
            screenshots.Add(new GameScreenshots
            {
                GameInfo = gameInfo,
                Picture = await FileToByteArrayAsync(screenshoot)
            });
        }
        return screenshots;
    }

    private async Task<List<GameFile>> ProcessAdditionalFiles(Game game, List<IFormFile> additionalFiles)
    {
        var gameFiles = new List<GameFile>();
        foreach (var gamefile in additionalFiles)
        {
            gameFiles.Add(new GameFile
            {
                Game = game,
                File = await FileToByteArrayAsync(gamefile)
            });
        }
        return gameFiles;
    }

    private GameInfo UpdateGameInfo(GameInfo gameInfo, GameViewModel vm)
    {
        gameInfo.Developer = vm.Developer;
        gameInfo.IsAdditionalFiles = Convert.ToBoolean(vm.IsAdditionalFiles);
        gameInfo.Description = vm.Description;
        gameInfo.Genre = vm.Genre;
        gameInfo.DiskNumber = vm.DiscNumber;
        gameInfo.ReleaseDate = DateOnly.Parse(vm.ReleaseDate);
        return gameInfo;
    }

    private async Task UpdateTitlePictureAsync(GameInfo gameInfo, IFormFile titlePictureFile)
    {
        var titlePicture = await context.GameTitlePicture.FirstOrDefaultAsync(tp => tp.GameInfoId == gameInfo.Id);
        if (titlePicture is null)
        {
            titlePicture = new GameTitlePicture
            {
                GameInfo = gameInfo,
                Picture = await FileToByteArrayAsync(titlePictureFile)
            };
            await context.GameTitlePicture.AddAsync(titlePicture);
        }
        else
        {
            titlePicture.Picture = await FileToByteArrayAsync(titlePictureFile);
            context.GameTitlePicture.Update(titlePicture);
        }
    }

    private async Task UpdateScreenshotsAsync(GameInfo gameInfo, List<IFormFile> screenshotFiles)
    {
        var existingScreenshots = await context.GameScreenshot
            .Where(gs => gs.GameInfoId == gameInfo.Id)
            .ToListAsync();

        // Delete excess screenshots
        if (existingScreenshots.Count > screenshotFiles.Count)
        {
            var screenshotsToRemove = existingScreenshots.Skip(screenshotFiles.Count).ToList();
            context.GameScreenshot.RemoveRange(screenshotsToRemove);
        }

        // Update existing screenshots or add new ones
        for (int i = 0; i < screenshotFiles.Count; i++)
        {
            if (i < existingScreenshots.Count)
            {
                existingScreenshots[i].Picture = await FileToByteArrayAsync(screenshotFiles[i]);
                context.GameScreenshot.Update(existingScreenshots[i]);
            }
            else
            {
                var newScreenshot = new GameScreenshots
                {
                    GameInfo = gameInfo,
                    Picture = await FileToByteArrayAsync(screenshotFiles[i])
                };
                await context.GameScreenshot.AddAsync(newScreenshot);
            }
        }
    }

    private async Task UpdateAdditionalFilesAsync(Game game, List<IFormFile> additionalFiles)
    {
        var existingFiles = await context.GameFile
            .Where(gf => gf.GameId == game.Id)
            .ToListAsync();

        // Delete excess files
        context.GameFile.RemoveRange(existingFiles);

        // Add new files
        var newFiles = new List<GameFile>();
        foreach (var file in additionalFiles)
        {
            newFiles.Add(new GameFile
            {
                Game = game,
                File = await FileToByteArrayAsync(file)
            });
        }
        await context.GameFile.AddRangeAsync(newFiles);
    }
}