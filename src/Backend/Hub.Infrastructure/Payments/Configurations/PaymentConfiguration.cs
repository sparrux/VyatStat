using Hub.Domain.Payments;
using Hub.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Payments.Configurations;

sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToPaymentsTable("payment", table =>
            table.HasCheckConstraint("ck_payment_amount_positive", "\"Amount\" > 0"));

        builder.ConfigurePaymentAggregate();
        builder.ConfigureMoney(payment => payment.Amount, "Amount", "Currency");

        builder.Ignore(payment => payment.RefundedAmount);
        builder.Ignore(payment => payment.RemainingRefundable);

        builder.Property(payment => payment.Purpose).AsStringEnum();
        builder.Property(payment => payment.Status).AsStringEnum();

        builder.Property(payment => payment.ReferenceId)
            .IsRequired();

        builder.Property(payment => payment.IdempotencyKey)
            .HasMaxLength(100);

        builder.Property(payment => payment.FailureReason)
            .HasMaxLength(1000);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(payment => payment.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasMany(payment => payment.Attempts)
            .WithOne(attempt => attempt.Payment)
            .HasForeignKey(attempt => attempt.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(payment => payment.Refunds)
            .WithOne(refund => refund.Payment)
            .HasForeignKey(refund => refund.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(payment => payment.Attempts, "_attempts");
        builder.ConfigureCollection(payment => payment.Refunds, "_refunds");

        builder.HasIndex(payment => payment.CustomerId);
        builder.HasIndex(payment => payment.Status);
        builder.HasIndex(payment => new { payment.Purpose, payment.ReferenceId })
            .IsUnique();

        builder.HasIndex(payment => payment.IdempotencyKey)
            .IsUnique()
            .HasFilter("\"IdempotencyKey\" IS NOT NULL");
    }
}
