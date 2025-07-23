using Snowball_Legacy.Domain.Common.Interfaces;

namespace Snowball_Legacy.Domain.Common;

public class BaseEntity : IEntity
{
    public int Id { get; set; }
}
