using Hub.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Payments.Configurations;

sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToPaymentsTable("invoice", table =>
            table.HasCheckConstraint("ck_invoice_amount_positive", "\"Amount\" > 0"));

        builder.ConfigurePaymentAggregate();
        builder.ConfigureMoney(invoice => invoice.Amount, "Amount", "Currency");
        builder.ConfigureOptionalPeriod(
            invoice => invoice.BillingPeriod,
            "PeriodStart",
            "PeriodEnd");

        builder.Property(invoice => invoice.DueDate)
            .IsRequired();

        builder.Property(invoice => invoice.Status).AsStringEnum();

        builder.Property(x => x.SourceId)
            .IsRequired(false);
        
        builder.Property(invoice => invoice.SourceType).AsStringEnum();

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(invoice => invoice.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne<Payment>()
            .WithMany()
            .HasForeignKey(invoice => invoice.PaymentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasIndex(invoice => invoice.CustomerId);
        builder.HasIndex(invoice => invoice.SourceId);
        builder.HasIndex(invoice => invoice.PaymentId);
        builder.HasIndex(invoice => invoice.Status);
        builder.HasIndex(invoice => invoice.DueDate);
    }
}
