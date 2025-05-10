using AtonTask.Domain.Models;

namespace AtonTask.Domain.Contracts.Repositories;

public interface IRefreshRepository
{
    Task<RefreshModel> CreateAsync(RefreshModel model, CancellationToken cancellationToken = default);
    Task<RefreshModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RefreshModel> UpdateAsync(RefreshModel model, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}