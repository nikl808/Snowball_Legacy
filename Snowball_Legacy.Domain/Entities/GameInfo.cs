using Snowball_Legacy.Domain.Common;

namespace Snowball_Legacy.Domain.Entities;

public sealed class GameInfo : BaseAuditableEntity
{
    public Game? Game { get; set; }
    public int GameId { get; set; }
    public int? DiskNumber { get; set; }
    public string? Genre { get; set; }
    public string? Developer { get; set; }
    public string? FromSeries { get; set; }
    public string? Description { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public bool IsAdditionalFiles { get; set; }
}
