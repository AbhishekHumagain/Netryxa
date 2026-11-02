using Microsoft.EntityFrameworkCore;
using Static.EntityFrameworkCore.Contracts.Auditing;
using System.Linq.Expressions;

namespace Static.EntityFrameworkCore.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="ModelBuilder" /> to apply common configurations to the entity model.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Applies a query filter for soft-deleted entities, ensuring that entities implementing <see cref="ISoftDeletable" />
        /// are excluded from queries by default unless explicitly included.
        /// This method also adds optional database indexing for the soft-delete column to improve query performance.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="ModelBuilder" /> instance to configure the model.</param>
        /// <param name="enableIndexing">Determines whether database indexes should be created for the <c>IsDeleted</c> property.</param>
        /// <param name="excludedEntities">A list of entity types to exclude from soft-delete indexing.</param>
        public static void ApplySoftDeleteQueryFilter(
            this ModelBuilder modelBuilder,
            bool enableIndexing = true,
            params Type[] excludedEntities)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            const string isDeletedPropertyName = nameof(ISoftDeletable.IsDeleted);
            var falseConstantValue = Expression.Constant(false);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                    continue;

                #region Apply soft-delete Query filter

                // Create the lambda expression: x => x.IsDeleted == false
                var parameter = Expression.Parameter(entityType.ClrType, "x");
                var propertyAccess = Expression.Property(parameter, isDeletedPropertyName);
                var equalExpression = Expression.Equal(propertyAccess, falseConstantValue);
                var lambda = Expression.Lambda(equalExpression, parameter);

                entityType.SetQueryFilter(lambda);

                #endregion

                #region Create Db Indexes

                // Add database index for the IsDeleted property if enabled and not excluded
                if (enableIndexing && !excludedEntities.Contains(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                                .HasIndex(isDeletedPropertyName)
                                .HasDatabaseName($"IX_{entityType.ClrType.Name}_IsNotDeleted")
                                .HasFilter($"\"{isDeletedPropertyName}\" = false"); // Postgres-compatible syntax
                }

                #endregion
            }
        }
    }
}