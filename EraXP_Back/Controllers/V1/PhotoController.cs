using EraXP_Back.Models;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Dto;
using EraXP_Back.Persistence;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[Route("api/v1/[Controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class PhotoController(
    IDbConnectionFactory connectionFactory,
    BlobStorage blobStorage
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> UploadPhotoAsync([FromForm] UniversityFileDto content)
    {
        Guid id = Guid.NewGuid();
        string path = await blobStorage.SaveFile(id, content.File);
        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Photo photo = new Photo(id, content.Name, path);
            await connection.Insert(photo);
        }
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotoAsync([FromQuery] Guid? imageId)
    {
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            List<Photo> photos = await connection.PhotoRepository.GetAll(id: imageId);

            return Ok(photos.Select(PhotoDto.From));
        }
    }
    
    [HttpGet]
    [Route("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetPhotoAsync([FromRoute] Guid id)
    {
        if (id == default)
            return BadRequest("Invalid photo id!");

        Stream stream;
        string filename;
        
        using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Photo? photo = await connection.PhotoRepository.Get(id: id);
            if (photo == null)
            {
                return BadRequest("Photo was not found!");
            }

            stream = await blobStorage.GetFile(photo.Uri);
            filename = photo.Name;
        }
        
        return File(stream, "image/png", filename); // returns a FileStreamResult
    }
}