using ContentFlow.Application.Interfaces.FileStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace ContentFlow.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _uploadPath;
    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    public FileStorageService(IConfiguration configuration)
    {
        _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
        Directory.CreateDirectory(_uploadPath);
    }
    
    public async Task<string> UploadAvatarAsync(int userId, IFormFile file, CancellationToken ct = default)
{
    if (file.Length == 0)
        throw new ArgumentException("File is empty.");
    
    if (file.Length > MaxFileSizeBytes)
        throw new ArgumentException($"Avatar size exceeds maximum of {MaxFileSizeBytes / (1024 * 1024)} MB.");

    // Проверка по Content-Type (можно подделать, но быстро отсеивает 90%)
    if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
        throw new ArgumentException("Only JPEG, PNG, GIF, and WebP images are allowed.");

    // Читаем файл ОДИН РАЗ в память
    byte[] fileBytes;
    using (var stream = file.OpenReadStream())
    {
        fileBytes = new byte[file.Length];
        await stream.ReadExactlyAsync(fileBytes, ct);
    }

    // Валидация через ImageSharp
    IImageFormat detectedFormat;
    try
    {
        using var imageStream = new MemoryStream(fileBytes);
        detectedFormat = await Image.DetectFormatAsync(imageStream, ct);
        if (detectedFormat == null)
            throw new ArgumentException("Unable to detect image format.");

        // Дополнительно: загрузим изображение, чтобы убедиться, что оно целое
        imageStream.Position = 0;
        using var _ = await Image.LoadAsync(imageStream, ct);
    }
    catch (Exception ex) when (ex is UnknownImageFormatException or NotSupportedException or ImageProcessingException)
    {
        throw new ArgumentException("Uploaded file is not a valid image.", ex);
    }

    // Определяем расширение
    var extension = detectedFormat.DefaultMimeType switch
    {
        "image/jpeg" => ".jpg",
        "image/png" => ".png",
        "image/gif" => ".gif",
        "image/webp" => ".webp",
        _ => ".jpg"
    };

    var fileName = $"{userId}_{Guid.NewGuid()}{extension}";
    var filePath = Path.Combine(_uploadPath, fileName);
    
    await File.WriteAllBytesAsync(filePath, fileBytes, ct);

    return $"/uploads/avatars/{fileName}";
}

    public async Task DeleteAvatarAsync(string avatarUrl, CancellationToken ct = default)
    {
        var fileName = Path.GetFileName(avatarUrl);
        var filePath = Path.Combine(_uploadPath, fileName);
        if (File.Exists(filePath))
            File.Delete(filePath);
        await Task.CompletedTask;
    }
}