using System.Security.Cryptography;
using AtonTask.Application.Dtos;
using AtonTask.Application.Mappers;
using AtonTask.Domain.Contracts.Repositories;
using AtonTask.Domain.Exceptions;

namespace AtonTask.Application.Services;

public class RefreshService(IRefreshRepository refreshRepository)
{
    public async Task<RefreshDto> CreateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var refresh = new RefreshDto
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiresIn = 30,
            Token = GenerateRefreshTokenString()
        };

        await refreshRepository.CreateAsync(refresh.ToModel(), cancellationToken);
        return refresh;
    }

    public async Task<RefreshDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var refresh = await refreshRepository.GetByIdAsync(id, cancellationToken);
        if (refresh is null)
        {
            throw new NotFoundException($"Рефреш с id {id} не найден");
        }

        return refresh.ToDto();
    }

    public async Task<RefreshDto> RefreshAsync(
        Guid userId,
        Guid refreshTokenId,
        string refreshToken,
        CancellationToken cancellationToken = default
    )
    {
        var refresh = (await refreshRepository.GetByIdAsync(refreshTokenId, cancellationToken))?.ToDto();
        if (refresh is null)
        {
            throw new AccessException($"Токен с id {refreshTokenId} не найден");
        }

        if (refresh.UserId != userId)
        {
            throw new AccessException($"Токен не принадлежит пользователю с id {userId}");
        }

        if (!string.Equals(refresh.Token, refreshToken))
        {
            throw new AccessException("Несоответствие рефреш токенов");
        }

        if (refresh.CreatedAt.AddDays(refresh.ExpiresIn).ToUniversalTime() < DateTime.UtcNow)
        {
            await refreshRepository.DeleteAsync(refreshTokenId, cancellationToken);
            throw new AccessException("Рефреш токен больше не действителен");
        }
        
        refresh.CreatedAt = DateTime.UtcNow;
        refresh.Token = GenerateRefreshTokenString();
        await refreshRepository.UpdateAsync(refresh.ToModel(), cancellationToken);
        
        return refresh;
    }

    private static string GenerateRefreshTokenString()
    {
        var refreshTokenBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(refreshTokenBytes);

        return Convert.ToBase64String(refreshTokenBytes);
    }
}