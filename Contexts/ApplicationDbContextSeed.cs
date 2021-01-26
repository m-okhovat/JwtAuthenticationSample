using JwtAuthenticationSample.Constants;
using JwtAuthenticationSample.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAuthenticationSample.Contexts
{
    public class ApplicationDbContextSeed
    {
        public static async Task SeedEssentialsAsync(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(AuthorizationConstants.Roles.Administrator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(AuthorizationConstants.Roles.Moderator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(AuthorizationConstants.Roles.User.ToString()));
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = AuthorizationConstants.DefaultUsername,
                Email = AuthorizationConstants.DefaultEmail,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                await userManager.CreateAsync(defaultUser, AuthorizationConstants.DefaultPassword);
                await userManager.AddToRoleAsync(defaultUser, AuthorizationConstants.DefaultRole.ToString());
            }
        }
    }
}