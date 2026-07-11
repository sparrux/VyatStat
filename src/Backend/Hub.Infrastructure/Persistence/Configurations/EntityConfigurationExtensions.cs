using System.Linq.Expressions;
using Hub.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Persistence.Configurations;

static class EntityConfigurationExtensions
{
    public static void ConfigureEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
    }

    public static void ConfigureAuditable<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Auditable
    {
        builder.ConfigureEntity();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();
    }

    public static NavigationBuilder ConfigureCollection<TEntity, TRelated>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, IEnumerable<TRelated>?>> navigationExpression,
        string fieldName)
        where TEntity : class
    {
        return builder
            .Navigation(navigationExpression)
            .HasField(fieldName)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
