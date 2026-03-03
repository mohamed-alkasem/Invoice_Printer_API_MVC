namespace Invoice_printer.Models
{
    public enum ReceiptType
    {
        Collection = 1,
        Payment = 2
    }

    public enum TemplateMode
    {
        Html = 1,
        Image = 2
    }

    public enum PaymentMethod
    {
        None = 0,
        Cash = 1,
        Card = 2,
        Transfer = 3,
        Other = 4
    }

    public enum ReceiptStatus
    {
        Draft = 1,
        Final = 2
    }

    public enum ExportFileType
    {
        Pdf = 1,
        Png = 2
    }
}
