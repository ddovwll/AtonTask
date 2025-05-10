using AtonTask.Application.Dtos;
using AtonTask.Domain.Models;

namespace AtonTask.Application.Mappers;

public static class RefreshMapper
{
    public static RefreshModel ToModel(this RefreshDto refreshDto)
    {
        return new RefreshModel
        {
            CreatedAt = refreshDto.CreatedAt,
            ExpiresIn = refreshDto.ExpiresIn,
            Id = refreshDto.Id,
            Token = refreshDto.Token,
            UserId = refreshDto.UserId,
        };
    }

    public static RefreshDto ToDto(this RefreshModel refreshModel)
    {
        return new RefreshDto
        {
            CreatedAt = refreshModel.CreatedAt,
            ExpiresIn = refreshModel.ExpiresIn,
            Id = refreshModel.Id,
            Token = refreshModel.Token,
            UserId = refreshModel.UserId,
        };
    }
}