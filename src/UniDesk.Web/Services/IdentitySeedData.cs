using Microsoft.AspNetCore.Identity;
using UniDesk.Web.Models;

namespace UniDesk.Web.Services
{
    public static class IdentitySeedData
    {
        public const string AdminRole = "Admin";
        public const string UserRole = "User";

        public const string AdminEmail = "admin@unidesk.pl";
        public const string AdminPassword = "Admin123!";

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await CreateRoleIfMissingAsync(roleManager, AdminRole);
            await CreateRoleIfMissingAsync(roleManager, UserRole);

            var admin = await userManager.FindByEmailAsync(AdminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    EmailConfirmed = true,
                    OrganizationName = "UniDesk Administration"
                };

                var createResult = await userManager.CreateAsync(admin, AdminPassword);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Nie udalo sie utworzyc konta administratora: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(admin, AdminRole))
            {
                await userManager.AddToRoleAsync(admin, AdminRole);
            }
        }

        private static async Task CreateRoleIfMissingAsync(
            RoleManager<IdentityRole> roleManager,
            string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Nie udalo sie utworzyc roli {roleName}: {errors}");
                }
            }
        }
    }
}