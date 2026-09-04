using Hub.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Payments.Configurations;

sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToPaymentsTable("subscription");

        builder.ConfigurePaymentAggregate();
        builder.ConfigureMoney(subscription => subscription.PriceSnapshot, "PriceSnapshot", "Currency");
        builder.ConfigureOptionalPeriod(
            subscription => subscription.CurrentPeriod,
            "PeriodStart",
            "PeriodEnd");

        builder.Property(subscription => subscription.PlanNameSnapshot)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(subscription => subscription.BillingInterval).AsStringEnum();
        builder.Property(subscription => subscription.Status).AsStringEnum();

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(subscription => subscription.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<SubscriptionPlan>()
            .WithMany()
            .HasForeignKey(subscription => subscription.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(subscription => subscription.CustomerId);
        builder.HasIndex(subscription => subscription.PlanId);
        builder.HasIndex(subscription => subscription.Status);
    }
}
