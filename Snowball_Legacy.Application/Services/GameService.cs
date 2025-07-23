using Snowball_Legacy.Application.DTOs;
using Snowball_Legacy.Application.Interfaces;
using Snowball_Legacy.Application.Interfaces.Repositories;
using Snowball_Legacy.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Snowball_Legacy.Application.Services;

public sealed class GameService(IGenericRepository<Game> gameRepository, IMemoryCache cache) : IGenericService<GameDto>
{
    private readonly IGenericRepository<Game> _gameRepository = gameRepository;
    private readonly IMemoryCache _cache = cache;
    private readonly MemoryCacheEntryOptions _cacheOptions = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };

    public async Task<IEnumerable<GameDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        // Не кэшируем список, только отдельные элементы
        var games = await _gameRepository.GetAllAsync(cancellationToken);
        var dtos = new List<GameDto>();
        foreach (var game in games)
        {
            dtos.Add(MapToGameDto(game));
        }
        return dtos;
    }

    public async Task<GameDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue($"Game_{id}", out var cachedObj) && cachedObj is GameDto cached)
            return cached;

        var game = await _gameRepository.GetByIdAsync(id, cancellationToken);
        var dto = game is null ? null : MapToGameDto(game);
        if (dto != null)
            _cache.Set($"Game_{id}", dto, _cacheOptions);
        return dto;
    }

    public async Task<GameDto?> AddAsync(GameDto gameDto, CancellationToken cancellationToken)
    {
        var newGame = await _gameRepository.AddAsync(new Game
        {
            Id = gameDto.Id,
            Name = gameDto?.Name ?? "New Game",
            Origin = gameDto?.Origin,
        }, cancellationToken);
        var dto = newGame is null ? null : MapToGameDto(newGame);
        if (dto != null)
            _cache.Set($"Game_{dto.Id}", dto, _cacheOptions);
        return dto;
    }

    public async Task UpdateAsync(GameDto gameDto, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(gameDto.Id, cancellationToken);
        if(game is null) throw new ArgumentException($"Game with id: {gameDto.Id} is not found");
        game.Name = gameDto.Name ?? "New Game";
        game.Origin = gameDto.Origin;
        await _gameRepository.UpdateAsync(game, cancellationToken);
        _cache.Remove($"Game_{gameDto.Id}");
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await _gameRepository.DeleteAsync(id, cancellationToken);
        _cache.Remove($"Game_{id}");
    }

    private GameDto MapToGameDto(Game game) =>
        new() { Id = game.Id, Name = game.Name, Origin = game.Origin };
}
