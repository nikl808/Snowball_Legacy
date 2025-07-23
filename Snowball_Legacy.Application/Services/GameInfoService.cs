using Snowball_Legacy.Application.DTOs;
using Snowball_Legacy.Application.Interfaces;
using Snowball_Legacy.Application.Interfaces.Repositories;
using Snowball_Legacy.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Snowball_Legacy.Application.Services;

public sealed class GameInfoService(IGenericRepositoryExt<GameInfo> gameInfoRepository, IGenericRepository<Game> gameRepository, IMemoryCache cache) : IGameInfoService<GameInfoDto>
{
    private readonly IGenericRepositoryExt<GameInfo> _gameInfoRepository = gameInfoRepository;
    private readonly IGenericRepository<Game> _gameRepository = gameRepository;
    private readonly IMemoryCache _cache = cache;
    private readonly MemoryCacheEntryOptions _cacheOptions = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };

    public async Task<IEnumerable<GameInfoDto>> GetAllAsync(CancellationToken cancellationToken) => await Task.FromResult(Enumerable.Empty<GameInfoDto>());
    
    public async Task<GameInfoDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue($"GameInfo_{id}", out var cachedObj) && cachedObj is GameInfoDto cached)
            return cached;
        var gameInfo = await _gameInfoRepository.GetByIdAsync(id, cancellationToken);
        if (gameInfo == null) return null;
        var game = await _gameRepository.GetByIdAsync(gameInfo.GameId, cancellationToken);
        var dto = game == null ? null : MapToGameInfoDto(gameInfo, game);
        if (dto != null)
            _cache.Set($"GameInfo_{id}", dto, _cacheOptions);
        return dto;
    }
    public async Task<GameInfoDto?> GetByForeignIdAsync(int id, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue($"GameInfo_{id}", out var cachedObj) && cachedObj is GameInfoDto cached)
            return cached;
        var gameInfo = await _gameInfoRepository.GetByIdAsync(g=>g.GameId == id, cancellationToken);
        if (gameInfo == null) return null;
        var game = await _gameRepository.GetByIdAsync(gameInfo.GameId, cancellationToken);
        return game == null ? null : MapToGameInfoDto(gameInfo, game);
    }

    public async Task<GameInfoDto?> AddAsync(GameInfoDto entity, CancellationToken cancellationToken)
    {
        var newGameInfo = await _gameInfoRepository.AddAsync(new GameInfo
        {
            Id = entity.Id,
            GameId = entity.GameId,
            Developer = entity.Developer,
            Genre = entity.Genre,
            ReleaseDate = DateOnly.Parse(entity.ReleaseDate), 
            FromSeries = entity.FromSeries,
            Description = entity.Description,
            DiskNumber = entity.DiscNumber,
            IsAdditionalFiles = entity.IsAdditionalFiles
        }, cancellationToken);
        if(newGameInfo is null) return null;

        var game = await _gameRepository.GetByIdAsync(entity.GameId, cancellationToken);
        var dto = game is not null ? MapToGameInfoDto(newGameInfo, game) : null;
        if (dto != null)
            _cache.Set($"GameInfo_{dto.Id}", dto, _cacheOptions);
        return dto;
    }

    public async Task UpdateAsync(GameInfoDto gameInfoDto, CancellationToken cancellationToken)
    {
        var gameInfo = await _gameInfoRepository.GetByIdAsync(gameInfoDto.Id, cancellationToken);
        if (gameInfo is null) throw new ArgumentException($"GameInfo with id: {gameInfoDto.Id} is not found");
        gameInfo.Developer = gameInfoDto.Developer;
        gameInfo.Genre = gameInfoDto.Genre;
        gameInfo.ReleaseDate = DateOnly.Parse(gameInfoDto.ReleaseDate);
        gameInfo.FromSeries = gameInfoDto.FromSeries;
        gameInfo.Description = gameInfoDto.Description;
        gameInfo.DiskNumber = gameInfoDto.DiscNumber;
        gameInfo.IsAdditionalFiles = gameInfoDto.IsAdditionalFiles;
        await _gameInfoRepository.UpdateAsync(gameInfo, cancellationToken);
        _cache.Remove($"GameInfo_{gameInfoDto.Id}");
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await _gameInfoRepository.DeleteAsync(id, cancellationToken);
        _cache.Remove($"GameInfo_{id}");
    }

    private GameInfoDto MapToGameInfoDto(GameInfo gameInfo, Game? game) =>
       new GameInfoDto
       {
           Id = gameInfo.Id,
           Name = game?.Name ?? string.Empty,
           Origin = game?.Origin ?? string.Empty,
           Developer = gameInfo.Developer ?? string.Empty,
           Genre = gameInfo.Genre ?? string.Empty,
           ReleaseDate = gameInfo.ReleaseDate?.ToString("dd-MM-yyyy") ?? string.Empty,
           FromSeries = gameInfo.FromSeries,
           Description = gameInfo.Description ?? string.Empty,
           DiscNumber = gameInfo.DiskNumber ?? 0,
           IsAdditionalFiles = gameInfo.IsAdditionalFiles
       };
}
