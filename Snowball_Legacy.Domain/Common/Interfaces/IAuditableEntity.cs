namespace Snowball_Legacy.Domain.Common.Interfaces;

public interface IAuditableEntity : IEntity
{
    int? CreatedBy { get; set; }
    int? UpdatedBy { get; set; }
    DateTime? CreatedDate { get; set; }
    DateTime? UpdatedDate { get; set; }
}
