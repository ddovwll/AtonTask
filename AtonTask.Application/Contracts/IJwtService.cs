using AtonTask.Application.Dtos;

namespace AtonTask.Application.Contracts;

public interface IJwtService
{
    string GenerateToken(UserDto user);
}