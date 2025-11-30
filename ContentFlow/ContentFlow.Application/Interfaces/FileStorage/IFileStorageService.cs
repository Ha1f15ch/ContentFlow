using Microsoft.AspNetCore.Http;

namespace ContentFlow.Application.Interfaces.FileStorage;

public interface IFileStorageService
{
    Task<string> UploadAvatarAsync(int userId, IFormFile file, CancellationToken ct = default);
    Task DeleteAvatarAsync(string avatarUrl, CancellationToken ct = default);
}