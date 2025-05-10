using AtonTask.Application.Dtos;
using AtonTask.Domain.Models;

namespace AtonTask.WebApi.Models.Responses;

public class GetByLoginResponse
{
    public required string Name { get; init; }
    public required Gender Gender { get; init; }
    public required DateTime? Birthday { get; init; }
    public required bool Active { get; init; }
}

public static class GetByLoginResponseMapper
{
    public static GetByLoginResponse ToResponse(this UserDto user)
    {
        return new GetByLoginResponse
        {
            Name = user.Name,
            Gender = user.Gender,
            Birthday = user.Birthday,
            Active = user.RevokedOn is null
        };
    }
}