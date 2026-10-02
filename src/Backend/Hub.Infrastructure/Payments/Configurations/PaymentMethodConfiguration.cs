using Hub.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Payments.Configurations;

sealed class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToPaymentsTable("payment_method");

        builder.ConfigurePaymentAuditable();
        builder.ConfigureProviderName(method => method.Provider);

        builder.Property(method => method.ProviderPaymentMethodId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(method => method.Type).AsStringEnum();

        builder.Property(method => method.Brand)
            .HasMaxLength(50);

        builder.Property(method => method.LastFour)
            .HasMaxLength(4)
            .IsUnicode(false);

        builder.Property(method => method.IsDefault)
            .IsRequired();

        builder.HasIndex(method => method.CustomerId);

        builder.HasIndex(method => new { method.CustomerId, method.Provider, method.ProviderPaymentMethodId })
            .IsUnique();
    }
}
