using Hub.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Payments.Configurations;

sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToPaymentsTable("subscription_plan", table =>
            table.HasCheckConstraint("ck_subscription_plan_price_positive", "\"Price\" > 0"));

        builder.ConfigurePaymentAggregate();
        builder.ConfigureMoney(plan => plan.Price, "Price", "Currency");

        builder.Property(plan => plan.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(plan => plan.BillingInterval).AsStringEnum();

        builder.Property(plan => plan.EntitlementKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(plan => plan.IsAvailable)
            .IsRequired();

        builder.HasIndex(plan => plan.EntitlementKey);
        builder.HasIndex(plan => plan.IsAvailable);
    }
}
