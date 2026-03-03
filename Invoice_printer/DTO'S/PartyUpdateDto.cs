using System.ComponentModel.DataAnnotations;

namespace Invoice_printer.DTO_S
{
    public class PartyUpdateDto : PartyCreateDto
    {
        [Required]
        public int Id { get; set; }
    }
}
