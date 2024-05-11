using System.Security.Cryptography;
using System.Text;
using EraXP_Back.Models;
using EraXP_Back.Models.Database;

namespace EraXP_Back.Utils;

public class UserUtils()
{
    public const int MIN_PASSWORD_LENGTH = 10;
    public bool ValidatePassword(User user, string password)
    {
        return user.Base64HashedPassword == GetHashedPasswordAsBase64(password, user.SecurityStamp);
    }

    public string? CreatePassword(User user, string password, string password2)
    {
        if (password != password2)
            return "Password mismatch!";

        if (password.Length < MIN_PASSWORD_LENGTH)
            return $"Your password's length must be greater than {MIN_PASSWORD_LENGTH}!";

        user.SecurityStamp = Guid.NewGuid();

        user.Base64HashedPassword = GetHashedPasswordAsBase64(
            password, user.SecurityStamp);

        return null;
    }

    public string GetHashedPasswordAsBase64(string password, Guid securityStamp)
    {
        byte[] decodedPassword = Encoding.UTF8.GetBytes(password);
        HMACSHA512 hmacsha512 = new HMACSHA512(securityStamp.ToByteArray());

        return Convert.ToBase64String(
            hmacsha512.ComputeHash(
                decodedPassword));
    }
}