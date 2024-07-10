using System.Text.Json;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Domain;
using EraXP_Back.Models.Domain.Enum;
using EraXP_Back.Models.Dto;
using EraXP_Back.Persistence;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EraXP_Back.Controllers.V1;

[ApiController]
[Route("/api/v1/user/signup")]
public class UserSignupController(
    IDbConnectionFactory connectionFactory,
    UserUtils userUtils,
    CypherUtil cypherUtil,
    ClaimUtils claimUtils,
    AuthorityUtils authorityUtils
) : ControllerBase
{
    private Authority? _authority;
    private Authority? Authority => _authority ??= claimUtils.GetAuthority(User);
    
    [HttpGet]
    [Route("generate")]
    public async Task<ActionResult<string>> GenerateToken([FromQuery] UserType userType, [FromQuery] string? email)
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> user = await authorityUtils.ValidateAuthority(connection, Authority);

            if ((userType == UserType.Admin || userType == UserType.UniAdmin) && (!user.IsSuccess ||
                user.Entity!.UserType != UserType.Admin))
                return Unauthorized("You are not authorize to generate a token of that level!");
            
            DateTimeOffset expirationDate = DateTimeOffset.Now.AddMinutes(5);
            SignupTokenDto signupTokenDto = new SignupTokenDto(userType, email, expirationDate);
            string json = JsonSerializer.Serialize(signupTokenDto);
            string tokenStr = Base64UrlEncoder.Encode(
                cypherUtil.EncryptStringToBytes_Aes(json));
            
            return Ok(tokenStr);
        }
    }

    [HttpGet]
    [Route("config/{token}")]
    public async ValueTask<ActionResult<SignupTokenDto>> GetBrokenUpToken([FromRoute] string token)
    {
        var tokenResult = GetToken(token);

        if (!tokenResult.IsSuccess)
            return StatusCode(tokenResult.Code!.Value, tokenResult.Error);

        return tokenResult.Entity!;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("{tokenStr}")]
    public async Task<ActionResult> SignUp([FromRoute] string tokenStr, [FromBody] SignUpDto signUpDto)
    {
        if (signUpDto.Password != signUpDto.PasswordRepeat)
            return BadRequest("Password mismatch!");

        if (signUpDto.Password.Length < UserUtils.MIN_PASSWORD_LENGTH)
            return BadRequest($"Your password's length must be greater than {UserUtils.MIN_PASSWORD_LENGTH}!");
        
        Result<SignupTokenDto> tokenResult = GetToken(tokenStr);

        if (!tokenResult.IsSuccess)
            return StatusCode(tokenResult.Code!.Value, tokenResult.Error);

        SignupTokenDto token = tokenResult.Entity!;

        if (token.Email != null && token.Email != signUpDto.Email)
            return BadRequest("Invalid email provided!");

        if (!token.IsValid(signUpDto))
            return BadRequest(
                "A university member needs to provide information about the university they are in!");

        if (token.IsProfessor())
        {
            return await SignUpUniversityProfessor(token, signUpDto);
        }

        if (token.IsStudent())
        {
            return await SignUpUniversityStudent(token, signUpDto);
        }

        return await SignUpMember(token, signUpDto);
    }

    private Result<SignupTokenDto> GetToken(string token)
    {
        try
        {
            byte[] tokenBytes = Base64UrlEncoder.DecodeBytes(token);
            string json = cypherUtil.DecryptStringFromBytes_Aes(tokenBytes);
            SignupTokenDto? tokenDto = JsonSerializer.Deserialize<SignupTokenDto>(json);

            if (tokenDto == null)
                return (400, "Invalid token!");
            
            if (tokenDto.ExpirationDate < DateTimeOffset.Now)
                return (400, "Expired token!");
            
            return tokenDto;
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc);
            return (400, "Invalid token!");
        }
    }


    private async Task<ActionResult> SignUpMember(SignupTokenDto token, SignUpDto signUpDto)
    {
        await using IDbConnection connection = await connectionFactory.ConnectAsync();
        {
            User user = CreateUserFromSignUpDto(signUpDto, token.UserType);

            await connection.Insert(user);
        }

        return Ok();
    }
    
    private async Task<ActionResult> SignUpUniversityProfessor(SignupTokenDto token, SignUpDto signUpDto)
    {
        await using IDbConnection connection = await connectionFactory.ConnectAsync();
        await using IDbTransaction transaction = connection.BeginTransaction();
        {
            try
            {
                List<University> universities =
                    await connection.UniversityRepository.Get(id: signUpDto.UniInfoDto!.UniversityId);
                if (universities.Count == 0)
                    return BadRequest("The university you are member off does not exist in the database!");

                User user = CreateUserFromSignUpDto(signUpDto, token.UserType);

                ProfessorUniversityInfo userUniversityInfo = new ProfessorUniversityInfo(
                    Guid.NewGuid(),
                    user.Id,
                    signUpDto.UniInfoDto.UniversityId
                );
                
                await transaction.Insert(user);
                await transaction.Insert(userUniversityInfo);

                await transaction.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
    
    private async Task<ActionResult> SignUpUniversityStudent(SignupTokenDto token, SignUpDto signUpDto)
    {
        if (signUpDto.UniInfoDto!.DepartmentId.IsDefault())
            return BadRequest("You need to provide a department id for the student!");
        
        await using IDbConnection connection = await connectionFactory.ConnectAsync();
        await using IDbTransaction transaction = connection.BeginTransaction();
        {
            try
            {
                List<University> universities =
                    await connection.UniversityRepository.Get(id: signUpDto.UniInfoDto!.UniversityId);
                
                if (universities.Count == 0)
                    return BadRequest("The university you are member off does not exist in the database!");

                User user = CreateUserFromSignUpDto(signUpDto, token.UserType);

                StudentUniversityInfo userUniversityInfo = new StudentUniversityInfo(
                    Guid.NewGuid(),
                    user.Id,
                    signUpDto.UniInfoDto.UniversityId,
                    signUpDto.UniInfoDto.DepartmentId!.Value
                );
                
                await transaction.Insert(user);
                await transaction.Insert(userUniversityInfo);

                await transaction.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
    private User CreateUserFromSignUpDto(SignUpDto signUpDto, UserType userType)
    {
        Guid securityToken = Guid.NewGuid();
        Guid concurrencyToken = Guid.NewGuid();

        string password = userUtils.CreatePassword(ref securityToken, signUpDto.Password, signUpDto.PasswordRepeat);

        User user = new User(
            Guid.NewGuid(),
            signUpDto.Username,
            password,
            signUpDto.Username.ToUpperInvariant(),
            signUpDto.Email,
            signUpDto.Email.ToUpperInvariant(),
            userType,
            securityToken,
            concurrencyToken
        );
        return user;
    }
}