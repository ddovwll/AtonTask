using System.ComponentModel.DataAnnotations;
using AtonTask.Domain.Models;

namespace AtonTask.WebApi.Models.Requests;

public class UpdateRequest
{
    [RegularExpression("^[A-Za-zА-Яа-яЁё]+$")]
    public string? Name { get; init; }
    public Gender? Gender { get; init; }
    public DateTime? Birthday { get; init; }
}