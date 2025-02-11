namespace Snowball_Legacy.Server.Models;

public class Game
{
    public int Id { get; set; }
    public string? Name { get; set; }
    GameInfo? GameInfo { get; set; }
}
