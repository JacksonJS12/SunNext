using System.ComponentModel.DataAnnotations;

namespace SunNext.Web.ViewModels.Home;

public class ContactFormViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Message is required")]
    [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
    [MinLength(10, ErrorMessage = "Message must be at least 10 characters long")]
    public string Message { get; set; } = string.Empty;
}