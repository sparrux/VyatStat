using Hub.Domain.Payments;
using Hub.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Payments.Configurations;

sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToPaymentsTable("order", table =>
            table.HasCheckConstraint("ck_order_total_positive", "\"Total\" > 0"));

        builder.ConfigurePaymentAggregate();
        builder.ConfigureMoney(order => order.Total, "Total", "Currency");

        builder.Property(order => order.Status).AsStringEnum();

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Payment>()
            .WithMany()
            .HasForeignKey(order => order.PaymentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasMany(order => order.Items)
            .WithOne(item => item.Order)
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ConfigureCollection(order => order.Items, "_items");

        builder.HasIndex(order => order.CustomerId);
        builder.HasIndex(order => order.PaymentId);
        builder.HasIndex(order => order.Status);
    }
}
