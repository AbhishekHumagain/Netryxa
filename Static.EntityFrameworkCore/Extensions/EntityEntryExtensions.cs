using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Static.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension methods for <see cref="EntityEntry"/> to interact with owned entities in the entity state.
/// </summary>
public static class EntityEntryExtensions
{
    /// <summary>
    /// Checks if any owned entities related to the current entity have changed (added or modified).
    /// </summary>
    /// <param name="entry">The <see cref="EntityEntry"/> instance representing the entity being tracked by the context.</param>
    /// <returns>
    /// <c>true</c> if any owned entities have been added or modified; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            r.TargetEntry.State is EntityState.Added or EntityState.Modified);
}