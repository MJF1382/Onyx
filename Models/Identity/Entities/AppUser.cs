using Microsoft.AspNetCore.Identity;

namespace Onyx.Models.Identity.Entities
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
