namespace Invoice_printer.Iservives
{
    public interface IQrCodeService
    {
        string GeneratePngDataUrl(string text, int pixelsPerModule = 20);
    }
}