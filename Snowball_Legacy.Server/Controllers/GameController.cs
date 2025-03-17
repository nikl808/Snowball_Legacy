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
    public ActionResult<IList<GameDto>> GetGames() {
        try
        {
            var games = context.Game.Select(g => new GameDto
            {
                Id = g.Id,
                Name = g.Name
            }).ToList();
            return games.Count() > 0 ? Ok(games) : NotFound();
        }
        catch (Exception e)
        {
            logger.LogError("Error when receiving game list", e);
        }
        return BadRequest();
    }

    /// <summary>
    /// Getting the game
    /// </summary>
    /// <param name="gameId">Game Id</param>
    /// <returns>GameInfoDto</returns>
    [HttpGet("info/{gameId}", Name = "GetGameInfo")]
    public ActionResult<GameInfoDto> GetGameInfo(int gameId)
    {
        try
        {
            context.Game.Where(g => g.Id == gameId).Include(i => i.GameInfo).Load();
            var game = context.Game.FirstOrDefault();
            if (game is not null && game.GameInfo is not null)
            {
                var info = new GameInfoDto()
                {
                    Id = game.GameInfo.Id,
                    Name = game.Name,
                    Developer =  game.GameInfo.Developer,
                    Genre = game.GameInfo.Genre,
                    ReleaseDate = game.GameInfo.ReleaseDate,
                    IsAdditionalFiles = game.GameInfo.IsAdditionalFiles,
                    DiscNumber = game.GameInfo.DiskNumber,
                    Description = game.GameInfo.Description
                };
                return Ok(info);
            }
            return NotFound();
        }
        catch (Exception e)
        {
            logger.LogError("Error when receiving game info", e);
        }
        return BadRequest();
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
            var game = new Game { Name = vm.Name };
            var gameInfo = SetGameInfo(game, vm);
            game.GameInfo = gameInfo;

            if (vm.TitlePicture is not null)
            {
                var titlePicture = await SetTitlePictureAsync(gameInfo, vm.TitlePicture[0]);
                await context.GameTitlePicture.AddAsync(titlePicture);
            }

            if (vm.Screenshots is not null)
            {
                var screenshots = await SetScreenshotsAsync(gameInfo, vm.Screenshots);
                gameInfo.ScreenShoots = screenshots;
                await context.GameScreenshot.AddRangeAsync(screenshots);
            }

            await context.Game.AddAsync(game);
            await context.GameInfo.AddAsync(gameInfo);

            if (vm.AdditionalFiles is not null)
            {
                var gameFiles = await SetAdditionalFiles(game, vm.AdditionalFiles);
                await context.GameFile.AddRangeAsync(gameFiles);
            }
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError("Game additional error", ex);
            return StatusCode(500, "Internal server error");
        }
        return Ok("Uploaded");
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
            var game = context.Game.Find(vm.Id);
            var gameInfo = context.GameInfo.Find(vm.Id);
            if (game is null || gameInfo is null) return BadRequest();

            game.Name = vm.Name;
            context.Game.Update(game);
            gameInfo.Developer = vm.Developer;
            gameInfo.IsAdditionalFiles = Convert.ToBoolean(vm.IsAdditionalFiles);
            gameInfo.Description = vm.Description;
            gameInfo.Genre = vm.Genre;
            gameInfo.DiskNumber = vm.DiscNumber;
            gameInfo.ReleaseDate = DateOnly.Parse(vm.ReleaseDate);
            context.GameInfo.Update(gameInfo);
            
            if (vm.TitlePicture is not null)
            {
                context.GameTitlePicture.Where(g => g.GameInfoId == gameInfo.Id).Load();
                var titlePicture = context.GameTitlePicture.FirstOrDefault();
                if (titlePicture is not null)
                {
                    titlePicture.Picture = await FileToByteArrayAsync(vm.TitlePicture[0]);
                    context.GameTitlePicture.Update(titlePicture);
                }
            }

            if (vm.Screenshots is not null)
            {
                context.GameScreenshot.Where(g => g.GameInfoId == gameInfo.Id).Load();
                var screenshotsContext = context.GameScreenshot.ToList();
                if(screenshotsContext is not null)
                {
                    var screens = vm.Screenshots.Select(async s => await FileToByteArrayAsync(s)).Select(r => r.Result).ToList();
                    if(screenshotsContext.Count < screens.Count)
                    {
                        foreach (var scr in screens)
                            screenshotsContext.Add(new GameScreenshots { GameInfo = gameInfo, Picture = scr });
                    }
                    
                    else if (screenshotsContext.Count > screens.Count)
                        screenshotsContext = screenshotsContext.Take(screens.Count).ToList();
                
                    for (int i = 0; i <= screenshotsContext.Count - 1; i++)
                        screenshotsContext[i].Picture = screens[i];
                    
                    context.GameScreenshot.UpdateRange(screenshotsContext);
                }
                
            }

            if (vm.AdditionalFiles is not null)
            {
                context.GameFile.Where(g => g.GameId == game.Id).Load();
                var files = context.GameFile.FirstOrDefault();
                if (files is not null)
                {
                    files.File = await FileToByteArrayAsync(vm.AdditionalFiles[0]);
                    context.GameFile.UpdateRange(files);
                }
            }
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError("Game update error", ex);
            return StatusCode(500, "Internal server error");
        }
        return Ok("Updated");
    }

    /// <summary>
    /// Delete game
    /// </summary>
    /// <param name="gameId">GameId</param>
    /// <returns>Ok result or error</returns>
    [HttpDelete("delete/{gameId}", Name = "DeleteGame")]
    public async Task<IActionResult> DeleteGame(int gameId)
    {
        var game = context.Game.Where(g => g.Id == gameId).FirstOrDefault();
        if (game is not null)
        {
            context.Game.Remove(game);
            await context.SaveChangesAsync();
            return Ok($"Game: {gameId} is removed");
        }
        logger.LogError("Game is not found", gameId);
        return BadRequest();
    }

    private async Task<byte[]> FileToByteArrayAsync(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    private GameInfo SetGameInfo(Game game, GameViewModel vm)
    {
        return new GameInfo {
            Game = game,
            Developer = vm.Developer,
            IsAdditionalFiles = Convert.ToBoolean(vm.IsAdditionalFiles),
            Description = vm.Description,
            Genre = vm.Genre,
            DiskNumber = vm.DiscNumber,
            ReleaseDate = DateOnly.Parse(vm.ReleaseDate)
        };
    }

    private async Task<GameTitlePicture> SetTitlePictureAsync(GameInfo gameInfo, IFormFile titlePictureFile)
    {
        var titlePicture = new GameTitlePicture();
        titlePicture.GameInfo = gameInfo;
        titlePicture.Picture = await FileToByteArrayAsync(titlePictureFile);
        gameInfo.TitlePicture = titlePicture;
        return titlePicture;
    }

    private async Task<List<GameScreenshots>> SetScreenshotsAsync(GameInfo gameInfo, List<IFormFile> screenshotFiles)
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

    private async Task<List<GameFile>> SetAdditionalFiles(Game game, List<IFormFile> additionalFiles)
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
}
