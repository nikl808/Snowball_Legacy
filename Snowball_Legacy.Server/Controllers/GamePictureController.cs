using Microsoft.AspNetCore.Mvc;
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
            var picture = context.GameTitlePicture.FirstOrDefault(g => g.GameInfoId == gameInfoId);
            
            if (picture?.Picture is null)
            {
                logger.LogWarning($"No title picture found for GameInfoId: {gameInfoId}");
                return NotFound($"No title picture found for GameInfoId: {gameInfoId}");
            }

            //Return file as jpeg
            return File(picture.Picture, "image/jpeg");
        }
        catch (Exception e)
        {
            logger.LogError(e, $"An error occurred while retrieving the title picture for GameInfoId: {gameInfoId}");
            return StatusCode(500, "Internal server error");
        }
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
            // Get screenshots from the database
            var screenshots = context.GameScreenshot
                .Where(g => g.GameInfoId == gameInfoId && g.Picture != null)
                .ToList();

            if (!screenshots.Any())
            {
                logger.LogWarning($"No screenshots found for GameInfoId: {gameInfoId}");
                return NotFound($"No screenshots found for GameInfoId: {gameInfoId}");
            }

            // Create zip-archive
            using var memoryStream = new MemoryStream();
            using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                for (int i = 0; i < screenshots.Count; i++)
                {
                    var screenshot = screenshots[i];
                    var entry = zip.CreateEntry($"screenshot_{i + 1}.jpg");
                    if (screenshot.Picture == null)
                    {
                        logger.LogWarning($"Null picture encountered for screenshot with Id: {screenshot.Id}");
                        continue;
                    }
                    using var entryStream = entry.Open();
                    entryStream.Write(screenshot.Picture, 0, screenshot.Picture.Length);
                }
            }

            //Return zip-archive
            return File(memoryStream.ToArray(), "application/zip", "screenshots.zip");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while retrieving screenshots for GameInfoId: {gameInfoId}");
            return StatusCode(500, "Internal server error");
        }
    }    
}
