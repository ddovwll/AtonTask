using AtonTask.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtonTask.Persistence.Configurations;

public class RefreshConfiguration : IEntityTypeConfiguration<RefreshModel>
{
    public void Configure(EntityTypeBuilder<RefreshModel> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.RefreshList)
            .HasForeignKey(x => x.UserId);
    }
}