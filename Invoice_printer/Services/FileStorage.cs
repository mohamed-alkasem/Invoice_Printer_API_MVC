using Invoice_printer.Iservives;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

public class FileStorage(IWebHostEnvironment env) : IFileStorage
{
    public async Task<string> SaveImageAsync(IFormFile file, string folder)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowed = new HashSet<string> { ".png", ".jpg", ".jpeg", ".webp" };
        if (!allowed.Contains(ext))
            throw new InvalidOperationException("Only image files are allowed.");

        var root = Path.Combine(env.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(root);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(root, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{folder}/{fileName}";
    }

    public Task DeleteIfExistsAsync(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return Task.CompletedTask;

        var trimmed = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(env.WebRootPath, trimmed);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }
}
