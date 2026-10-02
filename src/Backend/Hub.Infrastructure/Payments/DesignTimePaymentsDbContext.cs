using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hub.Infrastructure.Payments;

public sealed class DesignTimePaymentsDbContext : IDesignTimeDbContextFactory<PaymentsDbContext>
{
    public PaymentsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PaymentsDbContext>()
            .UsePaymentsNpgsql("Host=localhost;Database=vt-tracker;Username=username;Password=password")
            .Options;

        return new PaymentsDbContext(options);
    }
}
