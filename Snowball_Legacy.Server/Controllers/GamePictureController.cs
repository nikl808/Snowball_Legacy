using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Snowball_Legacy.Server.Contexts;
using System.IO.Compression;

namespace Snowball_Legacy.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamePictureController(DataContext context,
    ILogger<GamePictureController> logger) : ControllerBase
{
    /// <summary>
    /// Getting title picture
    /// </summary>
    /// <param name="gameInfoId">GameInfoId</param>
    /// <returns>Picture File</returns>
    [HttpGet("title/{gameInfoId:int}", Name = "GetTitlePicture")]
    public ActionResult GetTitlePicture([FromRoute] int gameInfoId)
    {
        try
        {
            context.GameTitlePicture.Where(g => g.GameInfoId == gameInfoId).Load();
            var pictures = context.GameTitlePicture.ToList();
            var picture = pictures.Find(i => i.GameInfoId == gameInfoId);
            return picture?.Picture is null ? NotFound() : File(picture.Picture, "image/jpeg");
        }
        catch (Exception e)
        {
            logger.LogError("Error when receiving title picture", e);
        }
        return BadRequest();
    }


    /// <summary>
    /// Getting screenshots
    /// </summary>
    /// <param name="gameInfoId">GameInfoId</param>
    /// <returns>zip archive</returns>
    [HttpGet("screenshots/{gameInfoId:int}", Name = "GetScreenshots")]
    public IActionResult GetScreenshots([FromRoute] int gameInfoId)
    {
        try
        {
            context.GameScreenshot.Where(g => g.GameInfoId == gameInfoId).Load();
            using (var ms = new MemoryStream())
            {
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
            
                    var screens = context.GameScreenshot.Where(i=>i.GameInfoId == gameInfoId).ToList();
                    if (screens.Count == 0) return NotFound();
                    var screenIndex = 0;
                    screens.ForEach(file =>
                    {
                        if (file.Picture is not null)
                        {
                            var entry = zip.CreateEntry($"screenshot_{screenIndex++}.jpg");
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
        catch (Exception e)
        {
            logger.LogError("Error when receiving screenshots", e);
        }
        return BadRequest();
    }
}
