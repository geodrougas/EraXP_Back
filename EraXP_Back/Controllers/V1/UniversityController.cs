using System.Text.Json;
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
    ClaimUtils claimUtils,
    BlobStorage blobStorage
) : ControllerBase
{
    private Authority? _authority;
    private Authority? Authority => _authority ??= claimUtils.GetAuthority(User);
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> CreateUniversity([FromForm] UniversityForm form)
    {
        if (form.Name == null)
            return BadRequest("You need to provide a name for the university!");
        
        if (form.AddressName == null || form.GoogleLocationId == null || form.Latitude == default || form.Longitude == default)
            return BadRequest("You need to provide an address for the university!");
        
        if (form.Language == null || form.LanguagePoints == null)
            return BadRequest("You need to provide a language for the university!");

        List<UniversityLanguageLevel>? languageLevel = JsonSerializer.Deserialize<List<UniversityLanguageLevel>>(form.LanguagePoints);

        if (languageLevel == null)
            return BadRequest("You need to provide a language for the university!");
        
        foreach (var item in languageLevel)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                return BadRequest("Invalid language item");
            }
        }

        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Code!.Value, result.Error!);
            }

            int changes = 0;
            Photo? photo = null;
            if (form.Image != null)
            {
                Guid id = Guid.NewGuid();
                string uri = await blobStorage.SaveFile(id, form.Image);
                photo = new Photo(id, id.ToString(), uri);

                changes += await connection.Insert(photo);
            }

            University university = new University(
                Guid.NewGuid(),
                form.Name,
                form.Description ?? "",
                photo?.Id,
                JsonSerializer.Serialize(form.GetLanguage(languageLevel))
            );
            
            Address address = new Address(
                Guid.NewGuid(),
                university.Id,
                form.AddressName,
                form.GoogleLocationId,
                form.Latitude,
                form.Longitude
            );
            
            changes += await connection.Insert(university);
            changes += await connection.Insert(address);

            return Ok(changes);
        }
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("alt")]
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

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> AlterUniversity([FromBody] UniversityDto dto)
    {
        if (dto.Id == null)
            return BadRequest("You cannot update a university without it's id.");
        
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Code!.Value, result.Error!);
            }

            Guid addressId = dto.AddressDto.Id ?? Guid.NewGuid();

            List<University> universities = await connection.UniversityRepository.Get(id: dto.Id.Value);

            if (universities.Count == 0)
                return BadRequest("Invalid university id.");

            University university = universities[0] with
            {
                Language = JsonSerializer.Serialize(dto.Language)
            };

            int changes = await connection.Update(university);

            return Ok(changes);
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    [Route("{id}/photo/{photoId}")]
    public async Task<ActionResult<int>> DeleteUniversityPhoto([FromRoute] Guid id, [FromRoute] Guid photoId)
    {

        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            return await connection.UniversityRepository.DeleteUniversityImage(photoId);
        }
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("{id}/photo")]
    public async Task<ActionResult<int>> CreateUniversityPhoto([FromRoute] Guid id, [FromForm] UniversityCreationDto photoDto)
    {
        if (photoDto.Photo == null)
        {
            return BadRequest("You need to upload a file!");
        }
        
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
            
            int changes = 0;
            Guid photoId = Guid.NewGuid();
            string uri = await blobStorage.SaveFile(photoId, photoDto.Photo);
            Photo photo = new Photo(photoId, photoId.ToString(), uri);

            changes += await connection.Insert(photo);

            UniversityPhoto universityPhoto = new UniversityPhoto(
                Guid.NewGuid(),
                id,
                photo.Id,
                null
            );
            
            changes += await connection.Insert(universityPhoto);

            return changes;
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
    [AllowAnonymous]
    [Route("all")]
    public async Task<ActionResult<IEnumerable<UniversityDto>>> GetAllUniversities()
    {
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
             List<University> allUniversities = await connection.UniversityRepository.Get();
             List<UniversityDto> universityDtos = new List<UniversityDto>(allUniversities.Count);
             
             Result<User> result = await authorityUtils.ValidateAuthority(connection, Authority);

             IUserUniversityInfo? userUniversityInfo = null;
             if (result.IsSuccess)
             {
                 User user = result.Entity!;

                 Result<IUserUniversityInfo>? infoResult = null;
                 if (user.UserType == UserType.Professor || user.UserType == UserType.Student)
                 {
                     infoResult = await UserUtils.GetUserUniversityInfo(connection, user);

                     if (!infoResult.IsSuccess)
                         return StatusCode(infoResult.Code!.Value, infoResult.Error!);

                     userUniversityInfo = infoResult.Entity!;
                 }
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
        IUserUniversityInfo? userUniversityInfo, bool isComplete)
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
                .Get(uniId: university.Id))
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

            Result<IUserUniversityInfo>? infoResult = null;
            IUserUniversityInfo? userUniversityInfo = null;
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

            Result<IUserUniversityInfo>? infoResult = null;
            IUserUniversityInfo? userUniversityInfo = null;
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
            
            Result<IUserUniversityInfo> infoResult =
                await UserUtils.GetUserUniversityInfo(connection, user);

            if (!infoResult.IsSuccess)
                return StatusCode(infoResult.Code!.Value, infoResult.Error!);
            
            IUserUniversityInfo userUniversityInfo = infoResult.Entity!;
            
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

            Result<IUserUniversityInfo> userUniversityInfos =
                await UserUtils.GetUserUniversityInfo(connection, user);

            if (!userUniversityInfos.IsSuccess)
                return StatusCode(userUniversityInfos.Code!.Value, userUniversityInfos.Error!);
            
            IUserUniversityInfo userUniversityInfo = userUniversityInfos.Entity!;
            
            return await GetAllForUniversityConnected(connection, userUniversityInfo, userUniversityInfo.UniversityId);
        }
    }

    private async Task<ActionResult<UniversityDto>> GetAllForUniversityConnected(IDbConnection connection, IUserUniversityInfo? userUniversityInfo, Guid universityId)
    {
        List<University> university;
        
        university = await connection.UniversityRepository.Get(id: universityId);

        if (university.Count == 0)
            return NotFound();
        
        UniversityDto? universityDto = await FromUniversity(connection, university[0], userUniversityInfo, true);
        
        return Ok(universityDto);
    }

    private async Task<ActionResult<UniversityDto>> GetUniversityConnected(IDbConnection connection, IUserUniversityInfo? userUniversityInfo, Guid universityId)
    {
        List<University> university = await connection.UniversityRepository.Get(id: universityId);

        if (university.Count == 0)
            return NotFound();

        UniversityDto? universityDto = await FromUniversity(connection, university[0], userUniversityInfo, false);
        
        return Ok(universityDto);
    }
}