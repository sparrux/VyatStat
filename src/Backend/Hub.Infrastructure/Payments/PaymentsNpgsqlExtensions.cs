using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Hub.Infrastructure.Payments;

static class PaymentsNpgsqlExtensions
{
    public static DbContextOptionsBuilder UsePaymentsNpgsql(
        this DbContextOptionsBuilder options,
        string? connectionString)
    {
        options.UseNpgsql(connectionString, npgsql =>
        {
            npgsql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, PaymentsDbContext.Schema);
        });

        return options;
    }

    public static DbContextOptionsBuilder<TContext> UsePaymentsNpgsql<TContext>(
        this DbContextOptionsBuilder<TContext> options,
        string? connectionString)
        where TContext : DbContext
    {
        ((DbContextOptionsBuilder)options).UsePaymentsNpgsql(connectionString);
        return options;
    }
}
