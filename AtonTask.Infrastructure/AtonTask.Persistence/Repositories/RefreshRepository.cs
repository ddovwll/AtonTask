using AtonTask.Domain.Contracts.Repositories;
using AtonTask.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AtonTask.Persistence.Repositories;

public class RefreshRepository(AppDbContext dbContext) : IRefreshRepository
{
    public async Task<RefreshModel> CreateAsync(RefreshModel model, CancellationToken cancellationToken = default)
    {
        await dbContext.Refreshes.AddAsync(model, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return model;
    }

    public async Task<RefreshModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Refreshes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<RefreshModel> UpdateAsync(RefreshModel model, CancellationToken cancellationToken = default)
    {
        dbContext.Refreshes.Update(model);
        await dbContext.SaveChangesAsync(cancellationToken);
        return model;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await dbContext.Refreshes.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
    }
}