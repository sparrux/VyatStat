using Hub.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Payments.Configurations;

sealed class DonationConfiguration : IEntityTypeConfiguration<Donation>
{
    public void Configure(EntityTypeBuilder<Donation> builder)
    {
        builder.ToPaymentsTable("donation", table =>
            table.HasCheckConstraint("ck_donation_amount_positive", "\"Amount\" > 0"));

        builder.ConfigurePaymentAggregate();
        builder.ConfigureMoney(donation => donation.Amount, "Amount", "Currency");
        builder.ConfigureOptionalBusinessReference(donation => donation.Reference);

        builder.Property(donation => donation.IsAnonymous)
            .IsRequired();

        builder.Property(donation => donation.Status).AsStringEnum();

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(donation => donation.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne<Payment>()
            .WithMany()
            .HasForeignKey(donation => donation.PaymentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasIndex(donation => donation.CustomerId);
        builder.HasIndex(donation => donation.PaymentId);
        builder.HasIndex(donation => donation.Status);
    }
}
