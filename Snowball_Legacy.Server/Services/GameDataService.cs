using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Snowball_Legacy.Server.Contexts;
using Snowball_Legacy.Server.Models;
using Snowball_Legacy.Server.Models.Dtos;
using Snowball_Legacy.Server.Models.ViewModels;
using Snowball_Legacy.Server.Utils;

namespace Snowball_Legacy.Server.Services;

public class GameDataService(DataContext context, ILogger<GameDataService> logger)
{
    private readonly DataContext _context = context;
    private readonly ILogger<GameDataService> _logger = logger;
    public async Task<Result<List<GameDto>>> GetListOfGames()
    {
        try
        {
            var games = await _context.Game
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Origin = g.Origin
                }).ToListAsync();
            return games.Any() ? games : Result<List<GameDto>>.NotFound("No games found.");
        }
        catch (Exception ex)
        {
            return LogServerError(new List<GameDto>(), ex, "Error fetching all games");
        }
    }

    public async Task<Result<GameInfoDto>> GetGameInfo(int gameId)
    {
        try
        {
            var game = await _context.Game.Include(i => i.GameInfo).FirstOrDefaultAsync(g => g.Id == gameId);
            if (game?.GameInfo is null)
                return Result<GameInfoDto>.NotFound($"Game with Id {gameId} not found");

            return new GameInfoDto()
            {
                Id = game.GameInfo.Id,
                Name = game.Name,
                Origin = game.Origin,
                Developer = game.GameInfo.Developer,
                Genre = game.GameInfo.Genre,
                ReleaseDate = game.GameInfo.ReleaseDate,
                FromSeries = game.GameInfo.FromSeries,
                IsAdditionalFiles = game.GameInfo.IsAdditionalFiles,
                DiscNumber = game.GameInfo.DiskNumber,
                Description = game.GameInfo.Description
            };
        }
        catch (Exception ex)
        {
            return LogServerError(new GameInfoDto(), ex, $"Error when receiving game info for Id {gameId}");
        }
    }

    public async Task<Result<string>> AddGame(GameViewModel vm)
    {
        try
        {
            // Create a game entity
            var game = new Game { Name = vm.Name, Origin = vm.Origin };
            var gameInfo = ProccessGameInfo(game, vm);
            game.GameInfo = gameInfo;

            // Process title picture
            if (vm.TitlePicture?.Any() is true)
            {
                var titlePicture = await ProcessTitlePictureAsync(gameInfo, vm.TitlePicture[0]);
                await _context.GameTitlePicture.AddAsync(titlePicture);
            }

            // Process screenshots
            if (vm.Screenshots?.Any() is true)
            {
                var screenshots = await ProcessScreenshotsAsync(gameInfo, vm.Screenshots);
                gameInfo.ScreenShoots = screenshots;
                await _context.GameScreenshot.AddRangeAsync(screenshots);
            }

            // Add game and game info to the context
            await _context.Game.AddAsync(game);
            await _context.GameInfo.AddAsync(gameInfo);

            //Process additional files
            if (vm.AdditionalFiles?.Any() is true)
            {
                var gameFiles = await ProcessAdditionalFiles(game, vm.AdditionalFiles);
                await _context.GameFile.AddRangeAsync(gameFiles);
            }

            // Save changes to the db
            await _context.SaveChangesAsync();
            return "Game successfully uploaded.";
        }
        catch (Exception ex)
        {
            return LogServerError(string.Empty, ex, "Game additional error");
        }
    }

    public async Task<Result<string>> UpdateGame(GameViewModel vm)
    {
        try
        {
            //Get game and game info
            var game = await _context.Game.Include(g => g.GameInfo).FirstOrDefaultAsync(g => g.Id == vm.Id);
            if (game is null || game.GameInfo is null) return Result<string>.NotFound($"Game with Id {vm.Id} not found.");

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

            await _context.SaveChangesAsync();
            return "Game successfully updated";
        }
        catch (Exception ex)
        {
            return LogServerError(string.Empty, ex, $"An error occurred while updating the game with ID {vm.Id}.");
        }
    }

    public async Task<Result<string>> DeleteGame([FromHeader] int gameId)
    {
        var game = _context.Game.Where(g => g.Id == gameId).FirstOrDefault();
        if (game is not null)
        {
            _context.Game.Remove(game);
            await _context.SaveChangesAsync();
            return $"Game: {gameId} is removed";
        }
        return LogServerError(string.Empty, null, "Game is not found");
    }

    private Result<T> LogServerError<T>(T value, Exception? error, string msg)
    {
        _logger.LogError(error, msg);
        return Result<T>.InternalServerError("Game is not found");
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
            FromSeries = vm.FromSeries,
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
        gameInfo.FromSeries = vm.FromSeries;
        gameInfo.Genre = vm.Genre;
        gameInfo.DiskNumber = vm.DiscNumber;
        gameInfo.ReleaseDate = DateOnly.Parse(vm.ReleaseDate);
        return gameInfo;
    }

    private async Task UpdateTitlePictureAsync(GameInfo gameInfo, IFormFile titlePictureFile)
    {
        var titlePicture = await _context.GameTitlePicture.FindAsync(gameInfo.Id);
        if (titlePicture is null)
        {
            titlePicture = new GameTitlePicture
            {
                GameInfo = gameInfo,
                Picture = await FileToByteArrayAsync(titlePictureFile)
            };
            await _context.GameTitlePicture.AddAsync(titlePicture);
        }
        else
        {
            titlePicture.Picture = await FileToByteArrayAsync(titlePictureFile);
            _context.GameTitlePicture.Update(titlePicture);
        }
    }

    private async Task UpdateScreenshotsAsync(GameInfo gameInfo, List<IFormFile> screenshotFiles)
    {
        var existingScreenshots = await _context.GameScreenshot
            .Where(gs => gs.GameInfoId == gameInfo.Id)
            .ToListAsync();

        // Delete excess screenshots
        if (existingScreenshots.Count > screenshotFiles.Count)
        {
            var screenshotsToRemove = existingScreenshots.Skip(screenshotFiles.Count).ToList();
            _context.GameScreenshot.RemoveRange(screenshotsToRemove);
        }

        // Update existing screenshots or add new ones
        for (int i = 0; i < screenshotFiles.Count; i++)
        {
            if (i < existingScreenshots.Count)
            {
                existingScreenshots[i].Picture = await FileToByteArrayAsync(screenshotFiles[i]);
                _context.GameScreenshot.Update(existingScreenshots[i]);
            }
            else
            {
                var newScreenshot = new GameScreenshots
                {
                    GameInfo = gameInfo,
                    Picture = await FileToByteArrayAsync(screenshotFiles[i])
                };
                await _context.GameScreenshot.AddAsync(newScreenshot);
            }
        }
    }

    private async Task UpdateAdditionalFilesAsync(Game game, List<IFormFile> additionalFiles)
    {
        var existingFiles = await _context.GameFile
            .Where(gf => gf.GameId == game.Id)
            .ToListAsync();

        // Delete excess files
        _context.GameFile.RemoveRange(existingFiles);

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
        await _context.GameFile.AddRangeAsync(newFiles);
    }
}
