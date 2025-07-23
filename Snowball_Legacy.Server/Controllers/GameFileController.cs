using Microsoft.AspNetCore.Mvc;
using Snowball_Legacy.Application.DTOs;
using Snowball_Legacy.Application.Interfaces;
using Snowball_Legacy.Server.Utils;

namespace Snowball_Legacy.Server.Controllers;

public sealed class GameFileController(IGenericFileService<GameFilesDto> service, ILogger<GameFileController> logger) : ApiControllerBase
{
    private readonly IGenericFileService<GameFilesDto> _service = service;
    private readonly ILogger<GameFileController> _logger = logger;
  
    [HttpGet("{id}")]
    public async Task<ActionResult<GameFilesDto>> Get(int id, CancellationToken cancellationToken)
    {
        using var timeoutCts = TimeoutUtils.CreateTimeoutCts(cancellationToken);
        _logger.LogInformation("Getting game files by id: {Id}", id);
        try
        {

            var images = await _service.GetByIdAsync(id, timeoutCts.Token);
            if (images is null)
            {
                _logger.LogWarning("Game files not found: {Id}", id);
                return NotFound();
            }
            return Ok(images);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout occurred while deleting game with id: {Id}", id);
            return StatusCode(504, "Operation timed out");
        }
    }

    [HttpGet("forGame/{id}")]
    public async Task<ActionResult<GameFilesDto>> GetForGame(int id, CancellationToken cancellationToken)
    {
        using var timeoutCts = TimeoutUtils.CreateTimeoutCts(cancellationToken);
        _logger.LogInformation("Getting files for game id: {Id}", id);
        try
        {
            var game = await _service.GetByForeignIdAsync(id, timeoutCts.Token);
            if (game is null)
            {
                _logger.LogWarning("Files for game not found: {Id}", id);
                return NotFound();
            }
            return Ok(game);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout occurred while deleting game with id: {Id}", id);
            return StatusCode(504, "Operation timed out");
        }
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Add([FromForm] GameFilesDto dto, CancellationToken cancellationToken)
    {
        using var timeoutCts = TimeoutUtils.CreateTimeoutCts(cancellationToken);
        _logger.LogInformation("Adding new game files");
        try
        {
            var game = await _service.AddAsync(dto, timeoutCts.Token);
            return Ok(game);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout occurred while deleting game with id: {Id}", dto.Id);
            return StatusCode(504, "Operation timed out");
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateGame([FromForm] GameFilesDto dto, CancellationToken cancellationToken)
    {
        using var timeoutCts = TimeoutUtils.CreateTimeoutCts(cancellationToken);
        _logger.LogInformation("Updating game files with id: {Id}", dto.Id);
        try
        {
            await _service.UpdateAsync(dto, timeoutCts.Token);
            return NoContent();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout occurred while deleting game with id: {Id}", dto.Id);
            return StatusCode(504, "Operation timed out");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromHeader] int id, CancellationToken cancellationToken)
    {
        using var timeoutCts = TimeoutUtils.CreateTimeoutCts(cancellationToken);
        _logger.LogInformation("Deleting game files with id: {Id}", id);
        try
        {
            await _service.DeleteAsync(id, timeoutCts.Token);
            return NoContent();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout occurred while deleting game with id: {Id}", id);
            return StatusCode(504, "Operation timed out");
        }
    }
}
