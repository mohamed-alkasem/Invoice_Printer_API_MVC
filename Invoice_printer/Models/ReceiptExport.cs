using System.ComponentModel.DataAnnotations;

namespace Invoice_printer.Models
{
    public class ReceiptExport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReceiptId { get; set; }
        public Receipt Receipt { get; set; } = default!;

        [Required]
        public ExportFileType FileType { get; set; } 

        [Required]
        public string FilePath { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
