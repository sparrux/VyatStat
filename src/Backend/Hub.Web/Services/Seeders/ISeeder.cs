namespace Hub.Web.Services.Seeders;

public interface ISeeder
{
    Task Seed(CancellationToken cancellationToken);
}