namespace Snowball_Legacy.Application.DTOs;

public sealed class GameInfoDto
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string ReleaseDate { get; set; } = string.Empty;
    public string? FromSeries { get; set; }
    public string Description { get; set; } = string.Empty;
    public int DiscNumber { get; set; }
    public bool IsAdditionalFiles { get; set; }
}