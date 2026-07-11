using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hub.Infrastructure.Persistence;

public sealed class DesignTimeHubDbContextFactory : IDesignTimeDbContextFactory<HubDbContext>
{
    public HubDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<HubDbContext>()
            .UseNpgsql("Host=localhost;Database=vt-tracker;Username=postgres;Password=postgres")
            .Options;

        return new HubDbContext(options);
    }
}
