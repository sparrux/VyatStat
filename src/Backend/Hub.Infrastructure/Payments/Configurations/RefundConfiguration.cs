using Hub.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Payments.Configurations;

sealed class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.ToPaymentsTable("refund", table =>
            table.HasCheckConstraint("ck_refund_amount_positive", "\"Amount\" > 0"));

        builder.ConfigurePaymentAuditable();
        builder.ConfigureMoney(refund => refund.Amount, "Amount", "Currency");

        builder.Property(refund => refund.Status).AsStringEnum();

        builder.Property(refund => refund.ProviderRefundId)
            .HasMaxLength(200);

        builder.Property(refund => refund.FailureReason)
            .HasMaxLength(1000);

        builder.HasIndex(refund => refund.PaymentId);

        builder.HasIndex(refund => refund.ProviderRefundId)
            .IsUnique()
            .HasFilter("\"ProviderRefundId\" IS NOT NULL");
    }
}
