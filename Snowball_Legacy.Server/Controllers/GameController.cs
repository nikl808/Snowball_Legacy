using Microsoft.AspNetCore.Mvc;
using Snowball_Legacy.Server.Contexts;
using Snowball_Legacy.Server.Models;
using Snowball_Legacy.Server.Models.ViewModels;

namespace Snowball_Legacy.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController(DataContext context) : ControllerBase
{
   
    [HttpPost]
    public async Task<IActionResult> AddGame([FromForm] GameViewModel vm)
    {
        var game = new Game { Name = vm.Name };
        var gameInfo = new GameInfo {
            Game = game,
            Description = vm.Description,
            Genre = vm.Genre,
            DiskNumber = vm.DiskNumber,
            ReleaseDate = DateOnly.Parse(vm.ReleaseDate)
        };
        game.GameInfo = gameInfo;

        if(vm.TitlePicture is not null)
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

        return new OkResult();
    }

    private async Task<byte[]> FileToByteArrayAsync(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
