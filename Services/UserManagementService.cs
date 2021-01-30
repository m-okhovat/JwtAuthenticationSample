using JwtAuthenticationSample.Constants;
using JwtAuthenticationSample.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAuthenticationSample.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagementService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return $"No Accounts Registered with {model.Email}.";
            }

            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var roleExists = Enum.GetNames(typeof(AuthorizationConstants.Roles))
                    .Any(x => x.ToLower() == model.Role.ToLower());

                if (roleExists)
                {
                    var validRole = Enum.GetValues(typeof(AuthorizationConstants.Roles))
                        .Cast<AuthorizationConstants.Roles>()
                        .FirstOrDefault(x => x.ToString().ToLower() == model.Role.ToLower());

                    await _userManager.AddToRoleAsync(user, validRole.ToString());
                    return $"Added {model.Role} to user {model.Email}.";
                }

                return $"Role {model.Role} not found.";
            }

            return $"Incorrect Credentials for user {user.Email}.";
        }
        public async Task<string> RegisterAsync(RegisterModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);
            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Constants.AuthorizationConstants.DefaultRole.ToString());
                }

                return $"User Registered with username {user.UserName}";
            }
            return $"Email {user.Email} is already registered.";
        }
    }
}