using System.Linq.Expressions;
using AtonTask.Domain.Contracts.Repositories;
using AtonTask.Domain.Exceptions;
using AtonTask.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AtonTask.Persistence.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<UserModel> CreateAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.Users.AddAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return user;
        }
        catch (DbUpdateException e)
            when (e.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException($"Пользователь с именем {user.Login} уже существует", e);
        }
    }

    public async Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Guid == userId, cancellationToken);
        return user;
    }
    
    public async Task<List<UserModel>> GetAsync(
        Expression<Func<UserModel, bool>>? filter = null,
        Func<IQueryable<UserModel>, IOrderedQueryable<UserModel>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext.Users.AsNoTracking().AsQueryable();

        if (filter is not null)
            query = query.Where(filter);

        if (orderBy != null)
            query = query.OrderBy(x => x.Login);

        if (skip.HasValue)
            query = query.Skip(skip.Value);

        if (take.HasValue)
            query = query.Take(take.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<UserModel> UpdateAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        try
        {
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync(cancellationToken);
            return user;
        }
        catch (DbUpdateException e)
            when (e.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException($"Пользователь с именем {user.Login} уже существует", e);
        }
    }

    public async Task DeleteAsync(
        Expression<Func<UserModel, bool>> filter,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext.Users.Where(filter).ExecuteDeleteAsync(cancellationToken);
    }
}