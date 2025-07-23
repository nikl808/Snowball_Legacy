using Snowball_Legacy.Domain.Common.Interfaces;

namespace Snowball_Legacy.Domain.Common;

public class BaseAuditableEntity : BaseEntity, IAuditableEntity
{
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
