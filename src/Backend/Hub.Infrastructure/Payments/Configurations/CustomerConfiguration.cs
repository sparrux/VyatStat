using Hub.Domain.Payments;
using Hub.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Payments.Configurations;

sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToPaymentsTable("customer");

        builder.ConfigurePaymentAggregate();

        builder.HasMany(customer => customer.PaymentMethods)
            .WithOne(method => method.Customer)
            .HasForeignKey(method => method.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(customer => customer.PaymentMethods, "_paymentMethods");
    }
}
