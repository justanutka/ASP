using Microsoft.AspNetCore.Identity;

namespace UniDesk.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string OrganizationName { get; set; } = string.Empty;
    }
}