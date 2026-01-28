using Microsoft.AspNetCore.Identity;

namespace Autentication.Models
{
    public class Users : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
