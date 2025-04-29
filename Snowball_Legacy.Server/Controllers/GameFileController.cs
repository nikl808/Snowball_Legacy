using Microsoft.AspNetCore.Mvc;
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
    public IResult GetAdditionalFiles(int gameId)
    {
        try
        {
            var file = context.GameFile.FirstOrDefault(g => g.GameId == gameId);
            
            // Check if file is null
            if (file?.File is null)
            {
                logger.LogWarning($"No additional files found for GameId: {gameId}");
                return Results.NotFound($"No additional files found for GameId: {gameId}");
            }

            //Return file as zip
            return Results.File(file.File, "application/zip", "files.zip");
        }
        catch (Exception e)
        {
            logger.LogError(e, $"An error occurred while retrieving additional files for GameId: {gameId}");
            return Results.StatusCode(500);
        }
    }
}
