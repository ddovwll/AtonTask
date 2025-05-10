using AtonTask.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtonTask.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserModel>
{
    public void Configure(EntityTypeBuilder<UserModel> builder)
    {
        builder.HasKey(x => x.Guid);

        builder
            .HasMany(x => x.RefreshList)
            .WithOne(x => x.User)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => x.Login).IsUnique();
    }
}