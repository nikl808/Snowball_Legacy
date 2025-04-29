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
        (await _gameDataService.GetListOfGames()).Match(
            onSuccess: Results.Ok,
            onFailure: error => CheckError(error).Item2);


    /// <summary>
    /// Getting the game
    /// </summary>
    /// <param name="gameId">Game Id</param>
    /// <returns>GameInfoDto</returns>
    [HttpGet("info/{gameId}", Name = "GetGameInfo")]
    public async Task<IResult> GetGameInfo(int gameId) =>
        (await _gameDataService.GetGameInfo(gameId)).Match(
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
        (await _gameDataService.AddGame(vm)).Match(
            onSuccess: Results.Ok,
            onFailure: error => CheckError(error).Item2);

    /// <summary>
    /// Update game
    /// </summary>
    /// <param name="vm">GameViewModel</param>
    /// <returns>Ok result or error</returns>
    [HttpPost("update", Name = "UpdateGame")]
    [DisableRequestSizeLimit]
    public async Task<IResult> UpdateGame([FromForm] GameViewModel vm) =>
        (await _gameDataService.UpdateGame(vm)).Match(
            onSuccess: Results.Ok,
            onFailure: error => CheckError(error).Item2);

    /// <summary>
    /// Delete game
    /// </summary>
    /// <param name="gameId">GameId</param>
    /// <returns>Ok result or error</returns>
    [HttpDelete]
    public async Task<IResult> DeleteGame([FromHeader] int gameId) =>
        (await _gameDataService.DeleteGame(gameId)).Match(
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
}