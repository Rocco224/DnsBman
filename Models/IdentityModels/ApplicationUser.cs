using Microsoft.AspNetCore.Identity;

namespace DnsBman.Models.IdentityModels
{
    public class ApplicationUser : IdentityUser
    {
        public UsersApiKey ApiKey { get; set; }
    }
}
