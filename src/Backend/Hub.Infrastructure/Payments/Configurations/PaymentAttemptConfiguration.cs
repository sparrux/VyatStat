using Hub.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Payments.Configurations;

sealed class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
{
    public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
    {
        builder.ToPaymentsTable("payment_attempt");

        builder.ConfigurePaymentAuditable();
        builder.ConfigureProviderName(attempt => attempt.Provider);

        builder.Property(attempt => attempt.ProviderPaymentId)
            .HasMaxLength(200);

        builder.Property(attempt => attempt.AttemptNumber)
            .IsRequired();

        builder.Property(attempt => attempt.Status).AsStringEnum();

        builder.Property(attempt => attempt.FailureCode)
            .HasMaxLength(100);

        builder.Property(attempt => attempt.FailureMessage)
            .HasMaxLength(1000);

        builder.HasIndex(attempt => attempt.PaymentId);

        builder.HasIndex(attempt => new { attempt.PaymentId, attempt.AttemptNumber })
            .IsUnique();

        builder.HasIndex(attempt => new { attempt.Provider, attempt.ProviderPaymentId })
            .IsUnique()
            .HasFilter("\"ProviderPaymentId\" IS NOT NULL");
    }
}
