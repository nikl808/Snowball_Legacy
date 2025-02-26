using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Snowball_Legacy.Server.Contexts;
using Snowball_Legacy.Server.Models;
using Snowball_Legacy.Server.Models.Dtos;
using Snowball_Legacy.Server.Models.ViewModels;
using System.IO.Compression;

namespace Snowball_Legacy.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController(DataContext context) : ControllerBase
{
    [HttpGet("list", Name = "GetGames")]
    public ActionResult<IList<GameDto>> GetGames() {
        var games = context.Game.Select(g => new GameDto
        {
            Id = g.Id,
            Name = g.Name
        }).ToList();
        return games.Count() > 0 ? Ok(games) : NotFound();
    }

    [HttpGet("info/{gameId:int}", Name = "GetGameInfo")]
    public ActionResult<GameInfoDto> GetGameInfo([FromRoute] int gameId)
    {
        context.Game.Where(g => g.Id == gameId).Include(i => i.GameInfo).Load();
        var game = context.Game.FirstOrDefault();
        if (game is not null && game.GameInfo is not null)
        {
            var info = new GameInfoDto()
            {
                Id = game.GameInfo.Id,
                Name = game.Name,
                Genre = game.GameInfo.Genre,
                ReleaseDate = game.GameInfo.ReleaseDate,
                DiscNumber = game.GameInfo.DiskNumber,
                Description = game.GameInfo.Description
            };
            return Ok(info);
        }
        return NotFound();
    }

    [HttpGet("titlePicture/{gameInfoId:int}", Name="GetTitlePicture")]
    public IActionResult GetTitlePicture([FromRoute] int gameInfoId)
    {
        context.GameTitlePicture.Where(g => g.GameInfoId == gameInfoId).Load();
        var pictures = context.GameTitlePicture.ToList();
        return pictures[0].Picture is null ? NotFound() : File(pictures[0].Picture, "image/jpeg");
    }

    [HttpGet("screenshots/{gameInfoId:int}", Name = "GetScreenshots")]
    public IActionResult GetScreenshots([FromRoute] int gameInfoId)
    {
        context.GameScreenshot.Where(g => g.GameInfoId == gameInfoId).Load();
        using (var ms = new MemoryStream())
        {
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                var screens = context.GameScreenshot.ToList();
                if (screens.Count == 0) return NotFound();
                var random = new Random();
                screens.ForEach(file =>
                {
                    if (file.Picture is not null)
                    {
                        var entry = zip.CreateEntry($"screenshot_{random.Next(0, 9)}.jpg");
                        using (var fs = new MemoryStream(file.Picture))
                        using (var es = entry.Open())
                        {
                            fs.CopyTo(es);
                        }
                    }
                });
            }
            return File(ms.ToArray(), "application/zip", "screenshots.zip");
        }
    }

    [HttpGet("additionalFiles/{gameId:int}", Name = "GetAdditionalFiles")]
    public IActionResult GetAdditionalFiles([FromRoute] int gameId)
    {
        context.GameFile.Where(g => g.GameId == gameId).Load();
        var files = context.GameFile.FirstOrDefault();
        return files is null ? NotFound() : File(files.File, "application/zip", "files.zip");
    }


    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> AddGame([FromForm] GameViewModel vm)
    {
        var game = new Game { Name = vm.Name };
        var gameInfo = new GameInfo {
            Game = game,
            Developer = vm.Developer,
            IsAdditionalFiles = vm.IsAdditionalFiles,
            Description = vm.Description,
            Genre = vm.Genre,
            DiskNumber = vm.DiscNumber,
            ReleaseDate = DateOnly.Parse(vm.ReleaseDate)
        };
        game.GameInfo = gameInfo;

        if (vm.TitlePicture is not null)
        {
            var titlePicture = new GameTitlePicture();
            titlePicture.GameInfo = gameInfo;
            titlePicture.Picture = await FileToByteArrayAsync(vm.TitlePicture[0]);
            gameInfo.TitlePicture = titlePicture;
            context.GameTitlePicture.Add(titlePicture);
        }

        if (vm.Screenshots is not null)
        {
            var screenshots = new List<GameScreenshots>();
            foreach (var screenshoot in vm.Screenshots)
            {
                screenshots.Add(new GameScreenshots
                {
                    GameInfo = gameInfo,
                    Picture = await FileToByteArrayAsync(screenshoot)
                });
            }
            gameInfo.ScreenShoots = screenshots;
            context.GameScreenshot.AddRange(screenshots);
        }

        context.Game.Add(game);
        context.GameInfo.Add(gameInfo);

        if (vm.AdditionalFiles is not null)
        {
            var gameFiles = new List<GameFile>();
            foreach (var gamefile in vm.AdditionalFiles)
            {
                gameFiles.Add(new GameFile
                {
                    Game = game,
                    File = await FileToByteArrayAsync(gamefile)
                });
            }
            context.GameFile.AddRange(gameFiles);
        }

        try
        {
            context.SaveChanges();
        }
        catch (Exception ex) {
            Console.WriteLine(ex.ToString());
            return StatusCode(500, "Internal server error");
        }
        return Ok("Uploaded");
    }

    private async Task<byte[]> FileToByteArrayAsync(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
