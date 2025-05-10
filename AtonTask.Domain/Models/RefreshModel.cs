namespace AtonTask.Domain.Models;

public class RefreshModel
{
    public required Guid Id { get; init; }
    public required string Token { get; init; }
    public required Guid UserId { get; init; }
    public UserModel? User { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required int ExpiresIn {get; init; }
}