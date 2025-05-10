namespace AtonTask.Application.Dtos;

public class RefreshDto
{
    public required Guid Id { get; set; }
    public required string Token { get; set; }
    public required Guid UserId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required int ExpiresIn {get; set; }
}