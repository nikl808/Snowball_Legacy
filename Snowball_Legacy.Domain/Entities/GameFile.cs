using Snowball_Legacy.Domain.Common;

namespace Snowball_Legacy.Domain.Entities;

public sealed class GameFile : BaseAuditableEntity
{
    public int GameId { get; set; }
    public Game? Game { get; set; }
    public byte[]? Data { get; set; }
}
