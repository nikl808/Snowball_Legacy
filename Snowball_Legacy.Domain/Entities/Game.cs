using Snowball_Legacy.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Snowball_Legacy.Domain.Entities;

public sealed class Game : BaseAuditableEntity
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Origin { get; set; }
}
