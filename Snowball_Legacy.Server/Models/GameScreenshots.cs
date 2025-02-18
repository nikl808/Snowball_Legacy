namespace Snowball_Legacy.Server.Models;

public class GameScreenshots
{
    public int Id { get; set; }
    public int GameInfoId { get; set; }
    public GameInfo? GameInfo { get; set; }
    public byte[]? Picture { get; set; }
}
