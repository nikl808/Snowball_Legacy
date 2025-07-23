namespace Snowball_Legacy.Domain.Entities;

public sealed class GameImage
{
    public int Id { get; set; }
    public int GameImagesId { get; set; }
    public GameImages? GameImages { get; set; }
    public string? Name { get; set; }
    public byte[]? Data { get; set; }
}