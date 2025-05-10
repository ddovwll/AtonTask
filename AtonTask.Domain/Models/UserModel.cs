namespace AtonTask.Domain.Models;

public class UserModel
{
    public required Guid Guid { get; init; }
    public List<RefreshModel>? RefreshList { get; init; }
    public required string Login { get; init; }
    public required string Password { get; init; }
    public required string Salt { get; init; }
    public required string Name { get; init; }
    public required Gender Gender { get; init; }
    public DateTime? Birthday { get; init; }
    public required bool Admin { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTime ModifiedOn { get; init; }
    public required string ModifiedBy { get; init; }
    public DateTime? RevokedOn { get; init; }
    public string? RevokedBy { get; init; }
}

public enum Gender
{
    Female,
    Male,
    Unknown
}