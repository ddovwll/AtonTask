using System.ComponentModel.DataAnnotations;

namespace AtonTask.WebApi.Models.Requests;

public class ChangePasswordRequest
{
    [RegularExpression("^[A-Za-z0-9]+$")] 
    public required string Password { get; init; }
}