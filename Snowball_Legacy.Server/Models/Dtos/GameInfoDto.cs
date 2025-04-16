namespace Snowball_Legacy.Server.Models.Dtos;

public class GameInfoDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Developer { get; set; }
    public string Genre { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
    public int? DiscNumber { get; set; }
    public bool IsAdditionalFiles { get; set; }
    public string? Description { get; set; }
}
