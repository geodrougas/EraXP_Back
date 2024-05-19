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

    public string CreatePassword(Guid securityStamp, string password, string password2)
    {
        if (password != password2)
            throw new ArgumentException("The passwords provided did not match!");

        if (password.Length < MIN_PASSWORD_LENGTH)
            throw new ArgumentException("The password had invalid length!");

        securityStamp = Guid.NewGuid();

        return GetHashedPasswordAsBase64(
            password, securityStamp);
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