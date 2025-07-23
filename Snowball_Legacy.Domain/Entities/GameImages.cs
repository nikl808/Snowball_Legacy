using Snowball_Legacy.Domain.Common;

namespace Snowball_Legacy.Domain.Entities;

public sealed class GameImages : BaseAuditableEntity
{
    public int GameId { get; set; }
    public Game? Game { get; set; }
    public List<GameImage>? Images { get; set; }
}