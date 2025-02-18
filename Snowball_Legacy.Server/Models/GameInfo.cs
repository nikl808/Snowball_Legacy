namespace Snowball_Legacy.Server.Models;

public class GameInfo
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int? DiskNumber { get; set; }
    public string? Description { get; set; }
    public Game? Game { get; set; }
    GamePicture? TitlePicture { get; set; }
    List<GamePicture>? ScreenShoots { get; set; }
}