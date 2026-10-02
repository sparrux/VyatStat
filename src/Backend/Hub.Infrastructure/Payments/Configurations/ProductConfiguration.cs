using Hub.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hub.Infrastructure.Payments.Configurations;

sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToPaymentsTable("product", table =>
            table.HasCheckConstraint("ck_product_price_positive", "\"Price\" > 0"));

        builder.ConfigurePaymentAggregate();
        builder.ConfigureMoney(product => product.Price, "Price", "Currency");

        builder.Property(product => product.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.IsAvailable)
            .IsRequired();

        builder.HasIndex(product => product.Name);
        builder.HasIndex(product => product.IsAvailable);
    }
}
