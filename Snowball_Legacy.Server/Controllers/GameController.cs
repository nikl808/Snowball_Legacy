using Microsoft.AspNetCore.Mvc;
using Snowball_Legacy.Server.Models.ViewModels;
using Snowball_Legacy.Server.Services;
using Snowball_Legacy.Server.Utils;

namespace Snowball_Legacy.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController(GameDataService gameDataService) : ControllerBase
{
    private readonly GameDataService _gameDataService = gameDataService;
    
    /// <summary>
    /// Getting a list of games
    /// </summary>
    /// <returns>List of GameDto</returns>
    [HttpGet("list", Name = "GetListOfGames")]
    public async Task<IResult> GetListOfGames() =>
        (await ProcessOperationTimeout(_gameDataService.GetListOfGames(),15)).Match(
            onSuccess: Results.Ok, onFailure: error => CheckError(error).Item2);
    
    /// <summary>
    /// Getting the game
    /// </summary>
    /// <param name="gameId">Game Id</param>
    /// <returns>GameInfoDto</returns>
    [HttpGet("info/{gameId}", Name = "GetGameInfo")]
    public async Task<IResult> GetGameInfo(int gameId) =>
        (await ProcessOperationTimeout(_gameDataService.GetGameInfo(gameId),15)).Match(
            onSuccess: Results.Ok,
            onFailure: error => CheckError(error).Item2);

    /// <summary>
    /// Add game
    /// </summary>
    /// <param name="vm">GameViewModel</param>
    /// <returns>Ok result or error</returns>
    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IResult> AddGame([FromForm] GameViewModel vm) =>
        (await ProcessOperationTimeout(_gameDataService.AddGame(vm),15)).Match(
            onSuccess: Results.Ok,
            onFailure: error => CheckError(error).Item2);

    /// <summary>
    /// Update game
    /// </summary>
    /// <param name="vm">GameViewModel</param>
    /// <returns>Ok result or error</returns>
    [HttpPut("update")]
    [DisableRequestSizeLimit]
    public async Task<IResult> UpdateGame([FromForm] GameViewModel vm) =>
        (await ProcessOperationTimeout(_gameDataService.UpdateGame(vm), 15)).Match(
            onSuccess: Results.Ok,
            onFailure: error => CheckError(error).Item2);

    /// <summary>
    /// Delete game
    /// </summary>
    /// <param name="gameId">GameId</param>
    /// <returns>Ok result or error</returns>
    [HttpDelete]
    public async Task<IResult> DeleteGame([FromHeader] int gameId) =>
        (await ProcessOperationTimeout(_gameDataService.DeleteGame(gameId), 15)).Match(
            onSuccess: Results.Ok,
            onFailure: error => CheckError(error).Item2);

    private (ErrorResponse,IResult) CheckError(ErrorResponse error) =>
        error.ErrorType switch
        {
            ErrorType.NotFound => (error, Results.NotFound(error.Error)),
            ErrorType.Invalid => (error, Results.BadRequest(error.Error)),
            ErrorType.InternalServerError => (error, Results.StatusCode(500)),
            _ => (error, Results.Problem(error.Error))
        };

    private async Task<T> ProcessOperationTimeout<T>(Task<T> task, int timeoutSec)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSec));
        var opDelayTask = Task.Delay(Timeout.Infinite, cts.Token);
        var result = await Task.WhenAny(task, opDelayTask);
        if (result == opDelayTask) return (T)Results.Problem("operation timeout");
        return await task;
    }
}