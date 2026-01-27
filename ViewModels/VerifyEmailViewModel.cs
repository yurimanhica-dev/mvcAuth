using System.ComponentModel.DataAnnotations;

namespace Autentication.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string Email { get; set; }
    }
}