using System.ComponentModel.DataAnnotations;

namespace AtonTask.WebApi.Models.Requests;

public class CredentialsRequest
{
    public required string Login { get; init; }
    public required string Password { get; init; }
}