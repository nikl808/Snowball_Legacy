using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Snowball_Legacy.Server.Contexts;

namespace Snowball_Legacy.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameFileController(DataContext context,
    ILogger<GameFileController> logger) : ControllerBase
{
    /// <summary>
    /// Getting additional files
    /// </summary>
    /// <param name="gameId">GameId</param>
    /// <returns>zip archive</returns>
    [HttpGet("archive/{gameId}")]
    public IActionResult GetAdditionalFiles(int gameId)
    {
        try
        {
            context.GameFile.Where(g => g.GameId == gameId).Load();
            var files = context.GameFile.FirstOrDefault();
            return files is null ? NotFound() : File(files.File, "application/zip", "files.zip");
        }
        catch (Exception e)
        {
            logger.LogError("Error when receiving files", e);
        }
        return BadRequest();
    }
}
