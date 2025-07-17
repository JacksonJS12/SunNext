using System.ComponentModel.DataAnnotations;

namespace SunNext.Web.ViewModels.Account
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}