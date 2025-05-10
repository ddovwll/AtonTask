using System.Linq.Expressions;
using AtonTask.Domain.Models;

namespace AtonTask.Domain.Contracts.Repositories;

public interface IUserRepository
{
    Task<UserModel> CreateAsync(UserModel user, CancellationToken cancellationToken);

    Task<List<UserModel>> GetAsync(
        Expression<Func<UserModel, bool>>? filter = null,
        Func<IQueryable<UserModel>, IOrderedQueryable<UserModel>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default
    );
    
    Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserModel> UpdateAsync(UserModel user, CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Expression<Func<UserModel, bool>> filter,
        CancellationToken cancellationToken = default
    );
}