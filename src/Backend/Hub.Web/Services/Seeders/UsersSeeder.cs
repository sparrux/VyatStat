using Hub.Domain;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Web.Services.Seeders;

sealed class UsersSeeder(HubDbContext dbContext) : ISeeder
{
    public async Task Seed(CancellationToken ctk)
    {
        if (!await dbContext.Users.AnyAsync(x => x.Nickname == "john", cancellationToken: ctk))
            await dbContext.AddAsync(User.Create(Guid.NewGuid(), "john").Value, ctk);
        
        if (!await dbContext.Users.AnyAsync(x => x.Nickname == "sam", cancellationToken: ctk))
            await dbContext.AddAsync(User.Create(Guid.NewGuid(), "sam").Value, ctk);
        
        if (!await dbContext.Users.AnyAsync(x => x.Nickname == "barbara", cancellationToken: ctk))
            await dbContext.AddAsync(User.Create(Guid.NewGuid(), "barbara").Value, ctk);
        
        await dbContext.SaveChangesAsync(ctk);
    }
}