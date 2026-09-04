using Hub.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Payments.Configurations;

sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToPaymentsTable("order_item", table =>
        {
            table.HasCheckConstraint("ck_order_item_quantity_positive", "\"Quantity\" > 0");
            table.HasCheckConstraint("ck_order_item_unit_price_positive", "\"UnitPrice\" > 0");
        });

        builder.ConfigurePaymentEntity();
        builder.ConfigureMoney(item => item.UnitPriceSnapshot, "UnitPrice", "UnitPriceCurrency");
        builder.ConfigureMoney(item => item.LineTotal, "LineTotal", "LineTotalCurrency");

        builder.Property(item => item.ProductNameSnapshot)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(item => item.Quantity)
            .IsRequired();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(item => item.OrderId);
        builder.HasIndex(item => item.ProductId);
    }
}
