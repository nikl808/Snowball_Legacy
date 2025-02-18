namespace Snowball_Legacy.Server.Models;

public class GameInfo
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public string Genre { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
    public int? DiskNumber { get; set; }
    public string? Description { get; set; }
    public Game? Game { get; set; }
    public GameTitlePicture? TitlePicture { get; set; }
    public List<GameScreenshots>? ScreenShoots { get; set; }
}