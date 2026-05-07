using System.ComponentModel.DataAnnotations;

public class CreateTicketRequest
{
    [Required(ErrorMessage = "Tytuł jest wymagany")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Tytuł musi mieć od 3 do 100 znaków")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "Status jest wymagany")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Status musi mieć od 3 do 20 znaków")]
    public required string Status { get; set; }
}