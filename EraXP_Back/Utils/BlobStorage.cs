namespace EraXP_Back.Utils;

public class BlobStorage(
    string connectionString
)
{
    public async ValueTask<string> SaveFile(Guid id, IFormFile formFile)
    {
        // Upload a file to a service or store somewhere?
        if (!Directory.Exists(connectionString))
        {
            Directory.CreateDirectory(connectionString);
        }

        string image = Path.Combine(connectionString, $"{id}.png");

        await using (FileStream fileStream = File.Open(image, FileMode.Create, FileAccess.ReadWrite))
        {
            await formFile.CopyToAsync(fileStream);
        }

        return image;
    }

    public ValueTask<Stream> GetFile(string fileString)
    {
        return ValueTask.FromResult(File.Open(fileString, FileMode.Open) as Stream);
    }
}