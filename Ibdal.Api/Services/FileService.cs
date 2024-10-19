namespace Ibdal.Api.Services;

public static class FileService
{
    private static readonly string UploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

    static FileService()
    {
        if (!Directory.Exists(UploadsFolder))
        {
            Directory.CreateDirectory(UploadsFolder);
        }
    }
    
    public static string SaveFile(IFormFile image)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(UploadsFolder, fileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            image.CopyTo(fileStream);
        }

        return $"/images/{fileName}";
    }

    public static void DeleteFile(string relativeFilePath)
    {
        var filePath = Path.Combine(UploadsFolder, relativeFilePath);
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}