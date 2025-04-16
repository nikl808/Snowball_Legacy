namespace Snowball_Legacy.Server.Models;

public class GameFile
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public Game? Game { get; set; }
    public byte[]? File { get; set; }
}
