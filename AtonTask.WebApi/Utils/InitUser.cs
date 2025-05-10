using AtonTask.Domain.Models;
using AtonTask.Persistence;

namespace AtonTask.WebApi.Utils;

public static class InitUser
{
    public static async Task Admin(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
        if (!context.Users.Any())
        {
            await context.Users.AddAsync(new UserModel
            {
                Admin = true,
                Birthday = null,
                CreatedBy = "Admin",
                CreatedOn = DateTime.UtcNow,
                Gender = Gender.Unknown,
                Guid = Guid.NewGuid(),
                Login = "Admin",
                ModifiedBy = "Admin",
                ModifiedOn = DateTime.UtcNow,
                Name = "Admin",
                Password = "WIltoXsHCaq1xQkzhHytz1ppH7oQOK23mHHHHp42sq8=",
                Salt = "8Mmw/pVPpl9BH8MT8KOqWg==",
                RevokedBy = null,
                RevokedOn = null
            });
            
            await context.SaveChangesAsync();
        }
    }
}