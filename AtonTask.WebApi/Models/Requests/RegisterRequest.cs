using System.ComponentModel.DataAnnotations;
using AtonTask.Application.Dtos;
using AtonTask.Domain.Models;

namespace AtonTask.WebApi.Models.Requests;

public class RegisterRequest
{
    [RegularExpression("^[A-Za-z0-9]+$")] 
    public required string Login { get; init; }
    

    [RegularExpression("^[A-Za-z0-9]+$")] 
    public required string Password { get; init; }
    

    [RegularExpression("^[A-Za-zА-Яа-яЁё]+$")]
    public required string Name { get; init; }

    public required Gender Gender { get; init; }
    public required DateTime Birthday { get; init; }
    public required bool Admin { get; init; }
}

public static class RegisterMapper
{
    public static UserDto ToDto(this RegisterRequest request)
    {
        return new UserDto
        {
            Guid = Guid.NewGuid(),
            Login = request.Login,
            Password = request.Password,
            Salt = string.Empty,
            Name = request.Name,
            Gender = request.Gender,
            Admin = request.Admin,
            CreatedOn = default,
            CreatedBy = string.Empty,
            ModifiedOn = default,
            ModifiedBy = string.Empty,
            Birthday = request.Birthday,
            RevokedBy = null,
            RevokedOn = null
        };
    }
}