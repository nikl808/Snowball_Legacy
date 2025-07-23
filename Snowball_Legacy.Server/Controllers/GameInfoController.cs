using Microsoft.AspNetCore.Mvc;
using Snowball_Legacy.Application.DTOs;
using Snowball_Legacy.Application.Interfaces;
using Snowball_Legacy.Server.Utils;

namespace Snowball_Legacy.Server.Controllers;

public sealed class GameInfoController(IGenericService<GameInfoDto> service, ILogger<GameInfoController> logger) : ApiControllerBase
{
    private readonly IGenericService<GameInfoDto> _service = service;
    private readonly ILogger<GameInfoController> _logger = logger;

    /// <summary>
    /// Getting the game information
    /// </summary>
    /// <param name="id">Game Id</param>
    /// <param name="cancellationToken">cancel token</param>
    /// <returns>GameInfoDto</returns>
    [HttpGet("info/{gameId}")]
    public async Task<ActionResult<GameInfoDto>> GetInfo(int id, CancellationToken cancellationToken)
    {
        using var timeoutCts = TimeoutUtils.CreateTimeoutCts(cancellationToken);
        _logger.LogInformation("Getting game info by id: {Id}", id);
        try
        {
            var result = await _service.GetByIdAsync(id, timeoutCts.Token);
            if (result is null)
            {
                _logger.LogWarning("Game info not found: {Id}", id);
                return NotFound();
            }
            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout occurred while deleting game with id: {Id}", id);
            return StatusCode(504, "Operation timed out");
        }
    }

    /// <summary>
    /// Add new game information
    /// </summary>
    /// <param name="dto">GameInfoDto</param>
    /// <param name="cancellationToken">cancel token</param>
    /// <returns>Ok result or error</returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromForm] GameInfoDto dto, CancellationToken cancellationToken)
    {
        using var timeoutCts = TimeoutUtils.CreateTimeoutCts(cancellationToken);
        _logger.LogInformation("Adding new game info");
        try
        {
            var result = await _service.AddAsync(dto, timeoutCts.Token);
            if (result is null)
            {
                _logger.LogWarning("Failed to add game info");
                return BadRequest("Failed to add game information");
            }
            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout occurred while deleting game with id: {Id}", dto.Id);
            return StatusCode(504, "Operation timed out");
        }
    }

    /// <summary>
    /// Update information about the game
    /// </summary>
    /// <param name="dto">GameInfoDto</param>
    /// <param name="cancellationToken">cancel token</param>
    /// <returns>Ok result or error</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromForm] GameInfoDto dto, CancellationToken cancellationToken)
    {
        using var timeoutCts = TimeoutUtils.CreateTimeoutCts(cancellationToken);
        _logger.LogInformation("Updating game info with id: {Id}", dto.Id);
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

    /// <summary>
    /// Delete information about the game
    /// </summary>
    /// <param name="id">GameId</param>
    /// <param name="cancellationToken">cancel token</param>
    /// <returns>Ok result or error</returns>
    [HttpDelete]
    public async Task<IActionResult> Delete([FromHeader] int id, CancellationToken cancellationToken)
    {
        using var timeoutCts = TimeoutUtils.CreateTimeoutCts(cancellationToken);
        _logger.LogInformation("Deleting game info with id: {Id}", id);
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
