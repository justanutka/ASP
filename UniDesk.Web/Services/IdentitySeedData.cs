using Microsoft.AspNetCore.Identity;
using UniDesk.Web.Models;
using System.Security.Claims;

namespace UniDesk.Web.Services
{
    public static class IdentitySeedData
    {
        public const string AdminRole = "Admin";
        public const string UserRole = "User";

        public const string AdminEmail = "admin@unidesk.pl";
        public const string AdminPassword = "Admin123!";

        public const string StudentEmail = "student@top-uni.edu.pl";
        public const string StudentPassword = "Student123!";
        public const string EmployeeIdClaimType = "EmployeeId";

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

            var student = await userManager.FindByEmailAsync(StudentEmail);

            if (student == null)
            {
                student = new ApplicationUser
                {
                    UserName = StudentEmail,
                    Email = StudentEmail,
                    EmailConfirmed = true,
                    OrganizationName = "Top Uni"
                };

                var createStudentResult = await userManager.CreateAsync(student, StudentPassword);

                if (!createStudentResult.Succeeded)
                {
                    var errors = string.Join("; ", createStudentResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Nie udalo sie utworzyc konta studenta: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(student, UserRole))
            {
                await userManager.AddToRoleAsync(student, UserRole);
            }

            await AddClaimIfMissingAsync(userManager, student, EmployeeIdClaimType, "EMP-1001");
            await AddClaimIfMissingAsync(userManager, admin, EmployeeIdClaimType, "EMP-0001");
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

        private static async Task AddClaimIfMissingAsync(
             UserManager<ApplicationUser> userManager,
             ApplicationUser user,
             string claimType,
             string claimValue)
        {
            var existingClaims = await userManager.GetClaimsAsync(user);

            if (!existingClaims.Any(c => c.Type == claimType && c.Value == claimValue))
            {
                await userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
            }
        }
    }
}