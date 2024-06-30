using System.Security.Cryptography;
using System.Text;
using EraXP_Back.Models;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Domain.Enum;
using EraXP_Back.Persistence;

namespace EraXP_Back.Utils;

public class UserUtils()
{
    public const int MIN_PASSWORD_LENGTH = 8;
    public bool ValidatePassword(User user, string password)
    {
        return user.Base64HashedPassword == GetHashedPasswordAsBase64(password, user.SecurityStamp);
    }

    public string CreatePassword(ref Guid securityStamp, string password, string password2)
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
    
    

    public static async Task<Result<IUserUniversityInfo>> GetUserUniversityInfo(IDbConnection connection, User user)
    {
        if (user.UserType == UserType.Professor)
        {
            ProfessorUniversityInfo? info = await connection.UserRepository.GetProfessorsUniversityInfo(userId: user.Id);

            if (info == null)
                return (500, "WTF Error! Failure to find the professor's connection to a university!");

            return info;
        }

        if (user.UserType == UserType.Student)
        {
            StudentUniversityInfo? info = await connection.UserRepository.GetStudentUniversityInfo(userId: user.Id);
            
            if (info == null)
                return (500, "WTF Error! Failure to find the student's connection to a university!");

            return info;
        }

        return (500, "Invalid type of user.");
    }
}