using AtonTask.Application.Dtos;
using AtonTask.Domain.Models;

namespace AtonTask.Application.Mappers;

public static class UserMapper
{
    public static UserDto ToUserDto(this UserModel userModel)
    {
        return new UserDto
        {
            Admin = userModel.Admin,
            Birthday = userModel.Birthday,
            CreatedBy = userModel.CreatedBy,
            CreatedOn = userModel.CreatedOn,
            Gender = userModel.Gender,
            Guid = userModel.Guid,
            Login = userModel.Login,
            ModifiedBy = userModel.ModifiedBy,
            ModifiedOn = userModel.ModifiedOn,
            Name = userModel.Name,
            Password = userModel.Password,
            RevokedBy = userModel.RevokedBy,
            RevokedOn = userModel.RevokedOn,
            Salt = userModel.Salt
        };
    }

    public static UserModel ToUserModel(this UserDto userDto)
    {
        return new UserModel
        {
            Admin = userDto.Admin,
            Birthday = userDto.Birthday,
            CreatedBy = userDto.CreatedBy,
            CreatedOn = userDto.CreatedOn,
            Gender = userDto.Gender,
            Guid = userDto.Guid,
            Login = userDto.Login,
            ModifiedBy = userDto.ModifiedBy,
            ModifiedOn = userDto.ModifiedOn,
            Name = userDto.Name,
            Password = userDto.Password,
            RevokedBy = userDto.RevokedBy,
            RevokedOn = userDto.RevokedOn,
            Salt = userDto.Salt
        };
    }
}