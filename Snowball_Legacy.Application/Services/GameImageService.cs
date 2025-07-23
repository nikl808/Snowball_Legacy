using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Caching.Memory;
using Snowball_Legacy.Application.DTOs;
using Snowball_Legacy.Application.Helpers;
using Snowball_Legacy.Application.Interfaces;
using Snowball_Legacy.Application.Interfaces.Repositories;
using Snowball_Legacy.Domain.Entities;

namespace Snowball_Legacy.Application.Services;

public sealed class GameImageService(IGenericRepositoryExt<GameImages> imagesRepository, IMemoryCache cache) : IGenericFileService<GameFilesDto>
{
    private readonly IGenericRepositoryExt<GameImages> _imagesRepository = imagesRepository;
    private readonly IMemoryCache _cache = cache;
    private readonly MemoryCacheEntryOptions _cacheOptions = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };

    public async Task<GameFilesDto?> GetByIdAsync(int id, CancellationToken token)
    {
        if(_cache.TryGetValue($"GameImages_{id}", out var cachedObj) && cachedObj is GameFilesDto cached)
            return cached;
        var images = await _imagesRepository.GetByIdAsync(id, token);
        var dto = images is null ? null : MapToGameFilesDto(images);
        if(dto is not null) _cache.Set($"GameImages_{id}", dto, _cacheOptions);
        return dto;
    }
    public async Task<GameFilesDto?> GetByForeignIdAsync(int id, CancellationToken token)
    {
        if (_cache.TryGetValue($"GameImages_{id}", out var cachedObj) && cachedObj is GameFilesDto cached)
            return cached;
        var images = await _imagesRepository.GetByIdAsync(g => g.GameId == id, token);
        if (images == null) return null;
        var game = await _imagesRepository.GetByIdAsync(images.GameId, token);
        var dto = game is null ? null : MapToGameFilesDto(images);
        if(dto is not null) _cache.Set($"GameImages_{id}", dto, _cacheOptions);
        return dto;
    }

    public async Task<GameFilesDto?> AddAsync(GameFilesDto dto, CancellationToken token)
    {
        var newImageFiles = await _imagesRepository.AddAsync(new GameImages
        {
            Id = dto.Id,
            GameId = dto.GameId,
            Images = dto.Files?.Select(file => new GameImage
            {
                Name = file.FileName,
                Data = FileHelper.FileToBytes(file)
            }).ToList()
        }, token);
        var result = newImageFiles is null ? null : MapToGameFilesDto(newImageFiles);
        if (result is not null) _cache.Set($"GameImages_{result.Id}", result, _cacheOptions);
        return result;
    }
    public async Task UpdateAsync(GameFilesDto dto, CancellationToken token)
    {
        var images = await _imagesRepository.GetByIdAsync(dto.Id, token);
        if (images is null) throw new ArgumentException($"Images with id: {dto.Id} is not found");

        images.Id = dto.Id;
        images.GameId = dto.GameId;
        images.Images = dto.Files?.Select(file => new GameImage
        {
            Name = file.FileName,
            Data = FileHelper.FileToBytes(file)
        }).ToList() ?? [];

        await _imagesRepository.UpdateAsync(images, token);
        _cache.Remove($"GameImages_{dto.Id}");
    }

    public async Task DeleteAsync(int id, CancellationToken token)
    {
        await _imagesRepository.DeleteAsync(id, token);
        _cache.Remove($"GameImages_{id}");
    }

    private GameFilesDto MapToGameFilesDto(GameImages images) =>
        new ()
        {
            Id = images.Id,
            GameId = images.GameId,
            Files = images.Images != null
                ? images.Images.Select(image => new FormFile(
                    new MemoryStream(image.Data ?? []),
                    0,
                    image.Data?.Length ?? 0,
                    image.Name,
                    image.Name)).ToList<IFormFile>()
                : []
        };
}
