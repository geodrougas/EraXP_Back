using EraXP_Back.Models;
using EraXP_Back.Models.Database;
using EraXP_Back.Models.Dto;
using EraXP_Back.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EraXP_Back.Controllers.V1;

[Route("api/v1/[Controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class PhotoController(
    IDbConnectionFactory connectionFactory
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> UploadPhotoAsync([FromForm] UniversityFileDto content)
    {
        string path = "./images";
        // Upload a file to a service or store somewhere?
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path = Path.Combine(path, $"{Guid.NewGuid()}.png");

        await using (FileStream fileStream = System.IO.File.Open(path, FileMode.Create, FileAccess.ReadWrite))
        {
            await content.File.CopyToAsync(fileStream);
        }

        await using (IDbConnection connection = await connectionFactory.ConnectAsync())
        {
            Photo photo = new Photo(Guid.NewGuid(), content.Name, path);
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

            stream = System.IO.File.Open(photo.Uri, FileMode.Open);
            filename = photo.Name;
        }
        
        return File(stream, "image/png", filename); // returns a FileStreamResult
    }
}