using System.Security.Cryptography;
using AtonTask.Application.Contracts;

namespace AtonTask.Infrastructure.Services;

public class EncryptService : IEncryptService
{
    public (string salt, string hash) HashPassword(string password)
    {
        var saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256))
        {
            var hashBytes = pbkdf2.GetBytes(32);
            return (Convert.ToBase64String(saltBytes), Convert.ToBase64String(hashBytes));
        }
    }

    public bool VerifyPassword(string password, string storedSalt, string storedHash)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);
        var storedHashBytes = Convert.FromBase64String(storedHash);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256);
        var computedHash = pbkdf2.GetBytes(32);
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
    }
}