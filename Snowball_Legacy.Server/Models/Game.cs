namespace Snowball_Legacy.Server.Models;

public class Game
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Origin { get; set; }
    public GameInfo? GameInfo { get; set; }
}
