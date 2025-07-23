using Microsoft.AspNetCore.Http;

namespace Snowball_Legacy.Application.DTOs;

public sealed class GameFilesDto
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public List<IFormFile>? Files { get; set; }
}
