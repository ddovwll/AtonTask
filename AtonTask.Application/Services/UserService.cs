using AtonTask.Application.Contracts;
using AtonTask.Application.Dtos;
using AtonTask.Application.Mappers;
using AtonTask.Domain.Contracts.Repositories;
using AtonTask.Domain.Exceptions;
using AtonTask.Domain.Models;

namespace AtonTask.Application.Services;

public class UserService(IUserRepository userRepository, IEncryptService encryptService, IJwtService jwtService)
{
    public async Task<UserDto> CreateAsync(
        UserDto user,
        string adminLogin,
        CancellationToken cancellationToken = default
    )
    {
        var encryptedData = encryptService.HashPassword(user.Password);

        user.Password = encryptedData.hash;
        user.Salt = encryptedData.salt;
        user.CreatedBy = adminLogin;
        user.CreatedOn = DateTime.UtcNow;
        user.ModifiedBy = adminLogin;
        user.ModifiedOn = user.CreatedOn;

        var savedUser = await userRepository.CreateAsync(user.ToUserModel(), cancellationToken);

        return savedUser.ToUserDto();
    }

    public async Task<UserDto> ChangePersonalDataAsync(
        Guid id,
        string modifiedBy,
        string? name,
        Gender? gender,
        DateTime? birthday,
        CancellationToken cancellationToken = default
    )
    {
        await CheckUpdateInitiatorAsync(modifiedBy, id, cancellationToken);
        var user = await GetUserAsync(id, cancellationToken);

        if (birthday is not null && birthday.Value.ToUniversalTime() > DateTime.UtcNow)
        {
            throw new ArgumentException("Некорректная дата рождения");
        }

        user.Name = name ?? user.Name;
        user.Gender = gender ?? user.Gender;
        user.Birthday = birthday ?? user.Birthday;
        user.ModifiedBy = modifiedBy;
        user.ModifiedOn = DateTime.UtcNow;

        var savedUser = await userRepository.UpdateAsync(user.ToUserModel(), cancellationToken);
        return savedUser.ToUserDto();
    }

    public async Task UpdatePasswordAsync(
        Guid id,
        string password,
        string modifiedBy,
        CancellationToken cancellationToken = default
    )
    {
        await CheckUpdateInitiatorAsync(modifiedBy, id, cancellationToken);
        var user = await GetUserAsync(id, cancellationToken);

        var encryptedData = encryptService.HashPassword(password);
        user.Password = encryptedData.hash;
        user.Salt = encryptedData.salt;
        user.ModifiedBy = modifiedBy;
        user.ModifiedOn = DateTime.UtcNow;

        await userRepository.UpdateAsync(user.ToUserModel(), cancellationToken);
    }

    public async Task UpdateLoginAsync(
        Guid id,
        string login,
        string modifiedBy,
        CancellationToken cancellationToken = default
    )
    {
        await CheckUpdateInitiatorAsync(modifiedBy, id, cancellationToken);
        var user = await GetUserAsync(id, cancellationToken);

        user.Login = login;
        user.ModifiedBy = modifiedBy;
        user.ModifiedOn = DateTime.UtcNow;

        await userRepository.UpdateAsync(user.ToUserModel(), cancellationToken);
    }

    public async Task<List<UserDto>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAsync(
            filter: x => x.RevokedOn == null,
            orderBy: x => x.OrderBy(y => y.CreatedOn),
            cancellationToken: cancellationToken);

        return users.Select(x => x.ToUserDto()).ToList();
    }

    public async Task<UserDto> GetByLoginAsync(
        string login,
        CancellationToken cancellationToken = default
    )
    {
        return await GetUserAsync(login, cancellationToken);
    }

    public async Task<UserDto> GetByCredentialsAsync(
        string login,
        string password,
        CancellationToken cancellationToken = default
    )
    {
        var user = await GetUserAsync(login, cancellationToken);
        if (!encryptService.VerifyPassword(password, user.Salt, user.Password))
        {
            throw new AccessException($"Неверные данные для входа в аккаунт");
        }

        return user;
    }

    public async Task<List<UserDto>> GetOlderThanAsync(
        int age,
        CancellationToken cancellationToken = default
    )
    {
        if (age < 0)
        {
            throw new ArgumentException("Некорректный возраст");
        }

        var users = await userRepository.GetAsync(
            filter: x => x.Birthday != null && DateTime.UtcNow.AddYears(-age) > x.Birthday.Value.ToUniversalTime(),
            cancellationToken: cancellationToken);

        return users.Select(x => x.ToUserDto()).ToList();
    }

    public async Task RevokeAsync(string userLogin, string adminLogin, CancellationToken cancellationToken = default)
    {
        var user = await GetUserAsync(userLogin, cancellationToken);

        user.RevokedBy = adminLogin;
        user.RevokedOn = DateTime.UtcNow;

        await userRepository.UpdateAsync(user.ToUserModel(), cancellationToken);
    }

    public async Task DeleteAsync(string login, CancellationToken cancellationToken = default)
    {
        await userRepository.DeleteAsync(x => x.Login == login, cancellationToken);
    }

    public async Task RestoreAsync(string login, string modifiedBy, CancellationToken cancellationToken = default)
    {
        var user = await GetUserAsync(login, cancellationToken);

        user.RevokedBy = null;
        user.RevokedOn = null;
        user.ModifiedOn = DateTime.UtcNow;
        user.ModifiedBy = modifiedBy;

        await userRepository.UpdateAsync(user.ToUserModel(), cancellationToken);
    }

    public async Task<(string token, Guid userId)> LogInAsync(
        string login,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await GetUserAsync(login, cancellationToken);

        if (!encryptService.VerifyPassword(password, user.Salt, user.Password))
        {
            throw new AccessException($"Неверные данные для входа в аккаунт");
        }

        var token = jwtService.GenerateToken(user);
        return (token, user.Guid);
    }

    private async Task<UserDto> GetUserAsync(string login, CancellationToken cancellationToken = default)
    {
        var user = (await userRepository.GetAsync(
            filter: x => string.Equals(x.Login, login),
            cancellationToken: cancellationToken)).FirstOrDefault()?.ToUserDto();
        if (user is null)
        {
            throw new NotFoundException($"Пользователь с логином {login} не найден");
        }

        return user;
    }

    public async Task<UserDto> GetUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = (await userRepository.GetByIdAsync(id, cancellationToken))?.ToUserDto();
        if (user is null)
        {
            throw new NotFoundException($"Пользователь с id {id} не найден");
        }

        return user;
    }

    public async Task<bool> CheckUserPermissionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = (await userRepository.GetByIdAsync(id, cancellationToken))?.ToUserDto();
        return user is not null && user.RevokedOn is null;
    }

    private async Task CheckUpdateInitiatorAsync(string modifiedBy, Guid id, CancellationToken cancellationToken = default)
    {
        var initiator = await GetUserAsync(modifiedBy, cancellationToken);
        if (!initiator.Admin && initiator.Guid != id)
        {
            throw new AccessException("Доступ к изменению данных запрещен");
        }
    }
}