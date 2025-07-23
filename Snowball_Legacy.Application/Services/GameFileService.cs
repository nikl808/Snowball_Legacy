using Snowball_Legacy.Application.DTOs;
using Snowball_Legacy.Application.Helpers;
using Snowball_Legacy.Application.Interfaces;
using Snowball_Legacy.Application.Interfaces.Repositories;
using Snowball_Legacy.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Snowball_Legacy.Application.Services;

public sealed class GameFileService(IGenericRepositoryExt<GameFile> gameFileRepository, IMemoryCache cache) : IGenericFileService<GameFilesDto>
{
    private readonly IGenericRepositoryExt<GameFile> _gameFileRepository = gameFileRepository;
    private readonly IMemoryCache _cache = cache;
    private readonly MemoryCacheEntryOptions _cacheOptions = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };

    
    public async Task<GameFilesDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue($"GameFile_{id}", out var cachedObj) && cachedObj is GameFilesDto cached)
            return cached;
        var files = await _gameFileRepository.GetByIdAsync(id, cancellationToken);
        var dto = files is null ? null : MapToGameFilesDto(files);
        if (dto is not null) _cache.Set($"GameFile_{id}", dto, _cacheOptions);
        return dto;
    }

    public async Task<GameFilesDto?> GetByForeignIdAsync(int id, CancellationToken cancellationToken)
    {
        if(_cache.TryGetValue($"GameFile_{id}", out var cachedObj) && cachedObj is GameFilesDto cached)
            return cached;
        var files = await _gameFileRepository.GetByIdAsync(f => f.GameId == id, cancellationToken);
        if (files is null) return null;
        var game = await _gameFileRepository.GetByIdAsync(files.GameId, cancellationToken);
        var dto = game is null ? null : MapToGameFilesDto(files);
        if (dto is not null) _cache.Set($"GameFile_{id}", dto, _cacheOptions);
        return dto;
    }

    public async Task<GameFilesDto?> AddAsync(GameFilesDto dto, CancellationToken cancellationToken)
    {
        var newFiles = await _gameFileRepository.AddAsync(new GameFile
        {
            Id = dto.Id,
            GameId = dto.GameId,
            Data = dto.Files?.Count > 0 ? FileHelper.FileToBytes(dto.Files[0]) : null
        }, cancellationToken);
        var result = newFiles is null ? null : MapToGameFilesDto(newFiles);
        if (result != null)
            _cache.Set($"GameFile_{result.Id}", result, _cacheOptions);
        return result;
    }

    public async Task UpdateAsync(GameFilesDto dto, CancellationToken cancellationToken)
    {
        var files = await _gameFileRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (files is null) throw new ArgumentException($"Files with id: {dto.Id} is not found");
        files.Id = dto.Id;
        files.GameId = dto.GameId;
        files.Data = dto.Files?.Count > 0 ? FileHelper.FileToBytes(dto.Files[0]) : null;
        await _gameFileRepository.UpdateAsync(files, cancellationToken);
        _cache.Remove($"GameFile_{dto.Id}");
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await _gameFileRepository.DeleteAsync(id, cancellationToken);
        _cache.Remove($"GameFile_{id}");
    }

    private GameFilesDto MapToGameFilesDto(GameFile gameFile) =>
       new GameFilesDto
       {
           Id = gameFile.Id,
           GameId = gameFile.GameId,
           Files = gameFile.Data != null ? FileHelper.BytesDictToFormFiles(new Dictionary<string, byte[]> { { "files", gameFile.Data } }) : []
       };
}