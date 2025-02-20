namespace Snowball_Legacy.Server.Models.Dtos;

public class GameInfoDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string Genre { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
    public int? DiskNumber { get; set; }
    public string? Description { get; set; }
}
