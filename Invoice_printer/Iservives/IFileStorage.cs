namespace Invoice_printer.Iservives
{
    public interface IFileStorage
    {
        Task<string> SaveImageAsync(IFormFile file, string folder);
        Task DeleteIfExistsAsync(string? relativePath);
    }

}
