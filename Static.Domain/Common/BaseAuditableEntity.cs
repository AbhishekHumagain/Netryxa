using Static.EntityFrameworkCore.Contracts.Auditing.Default;

namespace Static.Domain.Common;

public abstract class BaseAuditableEntity : Events, IAuditableEntity
{
    public virtual Guid Id { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? CreatorUserId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierUserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletionTime { get; set; }
    public Guid? DeleterUserId { get; set; }
}
