using EraXP_Back.Models;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Domain;
using EraXP_Back.Models.Domain.Enum;
using EraXP_Back.Models.Dto;
using EraXP_Back.Persistence;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EraXP_Back.Controllers.V1;

[Route("/api/v1/[Controller]")]
[ApiController]
public class UniversityController(
    IDbConnectionFactory connectionFactory,
    AuthorityUtils authorityUtils,
    ClaimUtils claimUtils
) : ControllerBase
{
    private Authority? _authority;
    private Authority? Authority => _authority ??= claimUtils.GetAuthority(User);
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> CreateUniversity([FromBody] UniversityDto dto)
    {
        if (dto.AddressDto == null)
            return BadRequest("You need to provide an address for the university!");
        
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Code!.Value, result.Error!);
            }

            Guid addressId = dto.AddressDto.Id ?? Guid.NewGuid();
            
            AddressDto addressDto = dto.AddressDto with { Id = addressId };
            
            University university = dto.To();
            Address address = addressDto.To(university.Id);

            int changes = await connection.Insert(university);
            changes += await connection.Insert(address);

            return Ok(changes);
        }
    }

    [HttpPost]
    [Route("{id}/photo")]
    public async Task<ActionResult<int>> CreateUniversityPhoto([FromRoute] Guid id, [FromBody] UniversityCreationDto photoDto)
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Code!.Value, result.Error!);
            }

            User user = result.Entity!;

            List<University> universities =
                await connection.UniversityRepository.Get(id: id);

            if (universities.Count == 0)
            {
                return BadRequest("University requested does not exist!");
            }

            if (photoDto.PhotoId != null)
            {
                return await SaveManagedUniversityPhoto(connection, id, photoDto.PhotoId.Value);
            }

            if (photoDto.Uri != null)
            {
                return await SaveUnmanagedUniversityPhoto(connection, id, photoDto.Uri);
            }

            return BadRequest("You need to provide either a managed or an unmanaged image to store!");
        }
    }

    private async Task<ActionResult> SaveManagedUniversityPhoto(IDbConnection connection, Guid universityId, Guid imageId)
    {
        Photo? photo = await connection.PhotoRepository.Get(id: imageId);

        if (photo == null)
            return BadRequest("Photo requested does not exist");

        UniversityPhoto uniPhoto = new UniversityPhoto(
            Guid.NewGuid(), universityId, imageId, null);

        int changes = await connection.Insert(uniPhoto);
        return Ok(changes);
    }

    private async Task<ActionResult> SaveUnmanagedUniversityPhoto(IDbConnection connection, Guid universityId, string uri)
    {
        UniversityPhoto uniPhoto = new UniversityPhoto(
            Guid.NewGuid(), universityId, null, uri);

        int changes = await connection.Insert(uniPhoto);
        return Ok(changes);
    }
    
    [HttpGet]
    [Route("all")]
    public async Task<ActionResult<IEnumerable<UniversityDto>>> GetAllUniversities()
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
             List<University> allUniversities = await connection.UniversityRepository.Get();
             List<UniversityDto> universityDtos = new List<UniversityDto>(allUniversities.Count);
             Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

             if (!result.IsSuccess)
             {
                 return StatusCode(result.Code!.Value, result.Error!);
             }

             User user = result.Entity!;

             Result<UserUniversityInfo>? infoResult = null;
             UserUniversityInfo? userUniversityInfo = null;
             if (user.UserType == UserType.Professor || user.UserType == UserType.Student)
             {
                 infoResult = await UserUtils.GetUserUniversityInfo(connection, user);

                 if (!infoResult.IsSuccess)
                     return StatusCode(infoResult.Code!.Value, infoResult.Error!);

                 userUniversityInfo = infoResult.Entity!;
             }
            
             
             foreach (var university in allUniversities)
             {
                 universityDtos.Add(
                     await FromUniversity(connection, university, userUniversityInfo, false));
             }
             
            return Ok(universityDtos);
        }
    }

    private async Task<UniversityDto> FromUniversity(IDbConnection connection, University university,
        UserUniversityInfo? userUniversityInfo, bool isComplete)
    {
        List<Address> addresses = await connection.UniversityRepository.GetAddress(uniId: university.Id);
        List<UniversityPhotoDto>? photos = null;
        List<DepartmentDto>? departments = null;

        if (isComplete)
        {
            string baseUrl;
            int? port = Request.Host.Port;
            if (port != null)
            {
                baseUrl = new UriBuilder(Request.Scheme, Request.Host.Host, port.Value).ToString();
            }
            else
            {
                baseUrl = new UriBuilder(Request.Scheme, Request.Host.Host).ToString();
            }

            photos = (await connection.UniversityRepository.GetPhoto(uniId: university.Id))
                .Select(m => UniversityPhotoDto.From(m, baseUrl)).ToList();

            departments = (await connection.DepartmentRepository
                .GetUniversityDepartments(uniId: university.Id))
                .Select(m => DepartmentDto.From(m)).ToList();
        }

        Guid? department = null;
        if (userUniversityInfo != null && university.Id == userUniversityInfo.UniversityId)
        {
            department = userUniversityInfo.DepartmentId;
        }

        return UniversityDto.From(
            university,
            addresses.FirstOrDefault(),
            department,
            photos,
            departments
        );
    }
    
    [HttpGet]
    [Route("{universityId}")]
    public async Task<ActionResult<UniversityDto>> GetUniversity([FromRoute] Guid universityId)
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Code!.Value, result.Error!);
            }

            User user = result.Entity!;

            Result<UserUniversityInfo>? infoResult = null;
            UserUniversityInfo? userUniversityInfo = null;
            if (user.UserType == UserType.Professor || user.UserType == UserType.Student)
            {
                infoResult = await UserUtils.GetUserUniversityInfo(connection, user);

                if (!infoResult.IsSuccess)
                    return StatusCode(infoResult.Code!.Value, infoResult.Error!);

                userUniversityInfo = infoResult.Entity!;
            }
            
            return await GetUniversityConnected(connection, userUniversityInfo, universityId);
        }
    }

    [HttpGet]
    [Route("{universityId}/complete")]
    public async Task<ActionResult<UniversityDto>> GetAllForUniversity([FromRoute] Guid universityId)
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Code!.Value, result.Error!);
            }

            User user = result.Entity!;

            Result<UserUniversityInfo>? infoResult = null;
            UserUniversityInfo? userUniversityInfo = null;
            if (user.UserType == UserType.Professor || user.UserType == UserType.Student)
            {
                infoResult = await UserUtils.GetUserUniversityInfo(connection, user);

                if (!infoResult.IsSuccess)
                    return StatusCode(infoResult.Code!.Value, infoResult.Error!);

                userUniversityInfo = infoResult.Entity!;
            }
            
            return await GetAllForUniversityConnected(connection, userUniversityInfo, universityId);
        }
    }


    [HttpGet]
    [Route("my")]
    [Authorize(Roles = "Student,Professor")]
    public async Task<ActionResult<UniversityDto>> GetMyUniversity()
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Code!.Value, result.Error!);
            }

            User user = result.Entity!;

            if (user.UserType != UserType.Professor && user.UserType != UserType.Student)
                return BadRequest("You are not part of a faculty!");
            
            Result<UserUniversityInfo> infoResult =
                await UserUtils.GetUserUniversityInfo(connection, user);

            if (!infoResult.IsSuccess)
                return StatusCode(infoResult.Code!.Value, infoResult.Error!);
            
            UserUniversityInfo userUniversityInfo = infoResult.Entity!;
            
            return await GetUniversityConnected(connection, userUniversityInfo, userUniversityInfo.UniversityId);
        }
    }
    
    [HttpGet]
    [Route("my/complete")]
    [Authorize(Roles = "Student,Professor")]
    public async Task<ActionResult<UniversityDto>> GetMyUniversityComplete()
    {
        University? university;
        
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Code!.Value, result.Error!);
            }

            User user = result.Entity!;

            Result<UserUniversityInfo> userUniversityInfos =
                await UserUtils.GetUserUniversityInfo(connection, user);

            if (!userUniversityInfos.IsSuccess)
                return StatusCode(userUniversityInfos.Code!.Value, userUniversityInfos.Error!);
            
            UserUniversityInfo userUniversityInfo = userUniversityInfos.Entity!;
            
            return await GetAllForUniversityConnected(connection, userUniversityInfo, userUniversityInfo.UniversityId);
        }
    }

    private async Task<ActionResult<UniversityDto>> GetAllForUniversityConnected(IDbConnection connection, UserUniversityInfo? userUniversityInfo, Guid universityId)
    {
        List<University> university;
        
        university = await connection.UniversityRepository.Get(id: universityId);

        if (university.Count == 0)
            return NotFound();
        
        UniversityDto? universityDto = await FromUniversity(connection, university[0], userUniversityInfo, true);
        
        return Ok(universityDto);
    }

    private async Task<ActionResult<UniversityDto>> GetUniversityConnected(IDbConnection connection, UserUniversityInfo? userUniversityInfo, Guid universityId)
    {
        List<University> university = await connection.UniversityRepository.Get(id: universityId);

        if (university.Count == 0)
            return NotFound();

        UniversityDto? universityDto = await FromUniversity(connection, university[0], userUniversityInfo, false);
        
        return Ok(universityDto);
    }
}