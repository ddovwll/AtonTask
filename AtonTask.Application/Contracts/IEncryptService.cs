namespace AtonTask.Application.Contracts;

public interface IEncryptService
{
    (string salt, string hash) HashPassword(string password);
    bool VerifyPassword(string password, string storedSalt, string storedHash);
}