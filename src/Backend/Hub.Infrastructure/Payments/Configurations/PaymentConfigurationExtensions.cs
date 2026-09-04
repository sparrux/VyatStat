using System.Linq.Expressions;
using Hub.Domain.Common;
using Hub.Domain.Payments.ValueObjects;
using Hub.Domain.ValueObjects;
using Hub.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Payments.Configurations;

static class PaymentConfigurationExtensions
{
    public static void ToPaymentsTable<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        string name)
        where TEntity : class
    {
        builder.ToTable(name, PaymentsDbContext.Schema);
    }

    public static void ToPaymentsTable<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        string name,
        Action<TableBuilder<TEntity>> configure)
        where TEntity : class
    {
        builder.ToTable(name, PaymentsDbContext.Schema, configure);
    }

    public static void ConfigurePaymentEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity
    {
        builder.ConfigureEntity();
        builder.Property(entity => entity.Id).ValueGeneratedNever();
    }

    public static void ConfigurePaymentAuditable<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Auditable
    {
        builder.ConfigureAuditable();
        builder.Property(entity => entity.Id).ValueGeneratedNever();
    }

    public static void ConfigurePaymentAggregate<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : AggregateRoot
    {
        builder.ConfigurePaymentAuditable();
        builder.Ignore(entity => entity.DomainEvents);
    }

    public static PropertyBuilder<TEnum> AsStringEnum<TEnum>(this PropertyBuilder<TEnum> builder)
        where TEnum : struct, Enum
    {
        return builder
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
    }

    public static void ConfigureMoney<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, Money?>> propertyExpression,
        string amountColumn,
        string currencyColumn)
        where TEntity : class
    {
        builder.ComplexProperty(propertyExpression, money =>
        {
            money.Ignore(value => value.IsZero);

            money.Property(value => value.Amount)
                .HasColumnName(amountColumn)
                .HasPrecision(18, 4)
                .IsRequired();

            money.ComplexProperty(value => value.Currency, currency =>
            {
                currency.Property(code => code.Code)
                    .HasColumnName(currencyColumn)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsRequired();
            });
        });
    }

    public static PropertyBuilder<ProviderName> ConfigureProviderName<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, ProviderName>> propertyExpression,
        string columnName = "Provider")
        where TEntity : class
    {
        return builder.Property(propertyExpression)
            .HasConversion(
                name => name.Value,
                value => ProviderName.Create(value).Value)
            .HasColumnName(columnName)
            .HasMaxLength(50)
            .IsRequired();
    }

    public static void ConfigureOptionalBusinessReference<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, BusinessReference?>> propertyExpression)
        where TEntity : class
    {
        builder.OwnsOne(propertyExpression, reference =>
        {
            reference.Property(value => value.Type)
                .HasColumnName("ReferenceType")
                .HasMaxLength(100);

            reference.Property(value => value.Id)
                .HasColumnName("ReferenceId");

            reference.HasIndex(value => new { value.Type, value.Id });
        });

        builder.Navigation(propertyExpression).IsRequired(false);
    }

    public static void ConfigureOptionalPeriod<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, DatesRange?>> propertyExpression,
        string startColumn,
        string endColumn)
        where TEntity : class
    {
        builder.ComplexProperty(propertyExpression, period =>
        {
            period.IsRequired(false);

            period.Property(value => value.StartDate)
                .HasColumnName(startColumn);

            period.Property(value => value.EndDate)
                .HasColumnName(endColumn);
        });
    }
}
