using Microsoft.AspNetCore.Mvc;
using Snowball_Legacy.Application.DTOs;
using Snowball_Legacy.Application.Interfaces;
using Snowball_Legacy.Server.Utils;

namespace Snowball_Legacy.Server.Controllers;

public sealed class GameController(IGenericService<GameDto> service, ILogger<GameController> logger) : ApiControllerBase
{
    private readonly IGenericService<GameDto> _service = service;
    private readonly ILogger<GameController> _logger = logger;
   
    /// <summary>
    /// Getting a list of games
    /// </summary>
    /// <returns>List of GameDto</returns>
    [HttpGet]
    public async Task<IEnumerable<GameDto>> GetAll(CancellationToken cancellationToken)
    {
        using var timeoutCts = TimeoutUtils.CreateTimeoutCts(cancellationToken);
        _logger.LogInformation("Getting all games");
        return await _service.GetAllAsync(timeoutCts.Token);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameDto>> GetById(int id, CancellationToken cancellationToken)
    {
        using var timeoutCts = TimeoutUtils.CreateTimeoutCts(cancellationToken);
        _logger.LogInformation("Getting game by id: {Id}", id);
        try
        {
            var game = await _service.GetByIdAsync(id, timeoutCts.Token);
            if (game is null)
            {
                _logger.LogWarning("Game not found: {Id}", id);
                return NotFound();
            }
            return Ok(game);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout occurred while getting game by id: {Id}", id);
            return StatusCode(504, "Operation timed out");
        }
       
    }

    [HttpPost]
    public async Task<IActionResult> AddGame([FromForm] GameDto gameDto, CancellationToken cancellationToken)
    {
        using var timeoutCts = TimeoutUtils.CreateTimeoutCts(cancellationToken);
        _logger.LogInformation("Adding new game");
        try
        {
            var game = await _service.AddAsync(gameDto, timeoutCts.Token);
            return Ok(game);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout occurred while adding game");
            return StatusCode(504, "Operation timed out");
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateGame([FromForm] GameDto gameDto, CancellationToken cancellationToken)
    {
        using var timeoutCts = TimeoutUtils.CreateTimeoutCts(cancellationToken);
        _logger.LogInformation("Updating game with id: {Id}", gameDto.Id);
        try
        {
            await _service.UpdateAsync(gameDto, timeoutCts.Token);
            return NoContent();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout occurred while updating game with id: {Id}", gameDto.Id);
            return StatusCode(504, "Operation timed out");
        }
    }

    /// <summary>
    /// Delete game
    /// </summary>
    /// <param name="id">GameId</param>
    /// <param name="cancellationToken">cancel token</param>
    /// <returns>Ok result or error</returns>
    [HttpDelete]
    public async Task<IActionResult> Delete([FromHeader] int id, CancellationToken cancellationToken)
    {
        using var timeoutCts = TimeoutUtils.CreateTimeoutCts(cancellationToken);
        _logger.LogInformation("Deleting game with id: {Id}", id);
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