using JwtAuthenticationSample.Constants;
using JwtAuthenticationSample.Contexts;
using JwtAuthenticationSample.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAuthenticationSample.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ITokenGenerator _tokenGenerator;

        public UserService(UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ITokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _context = context;
            _tokenGenerator = tokenGenerator;
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

        public async Task<AuthenticationResultModel> GenerateJwtByUserPass(AuthenticationRequestModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return AuthenticationResultModel.Failed($"No Accounts Registered with {model.Email}.");

            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var refreshToken = new RefreshToken();
                if (user.RefreshTokens.Any(a => a.IsActive))
                    refreshToken = user.RefreshTokens.FirstOrDefault(a => a.IsActive);
                else
                {
                    refreshToken = RefreshTokenGenerator.CreateRefreshToken();
                    user.RefreshTokens.Add(refreshToken);
                    _context.Update(user);
                    _context.SaveChanges();
                }

                return await GetAuthenticationResultModel(user, refreshToken);
            }

            return AuthenticationResultModel.Failed("Incorrect Credentials for user {user.Email}.");
        }


        public async Task<AuthenticationResultModel> GenerateJwtByRefreshToken(string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null)
                return AuthenticationResultModel.Failed($"Token did not match any users.");

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
                return AuthenticationResultModel.Failed($"Token Not Active.");

            //Revoke Current Refresh Token
            refreshToken.Revoked = DateTime.UtcNow;

            //Generate new Refresh Token and save to Database
            var newRefreshToken = RefreshTokenGenerator.CreateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            _context.Update(user);
            _context.SaveChanges();

            return await GetAuthenticationResultModel(user, newRefreshToken);
        }

        public ApplicationUser GetById(string id)
        {
            return _context.Users.Find(id);
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

        private async Task<AuthenticationResultModel> GetAuthenticationResultModel(ApplicationUser user,
            RefreshToken refreshToken)
        {
            //Generates new jwt

            var authenticationModel = new AuthenticationResultModel();
            authenticationModel.IsAuthenticated = true;
            JwtSecurityToken jwtSecurityToken = await _tokenGenerator.CreateJwtToken(user);
            authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authenticationModel.Email = user.Email;
            authenticationModel.UserName = user.UserName;
            var rolesList = await _userManager
                .GetRolesAsync(user).ConfigureAwait(false);
            authenticationModel.Roles = rolesList.ToList();
            authenticationModel.RefreshToken = refreshToken;
            return authenticationModel;
        }

    }
}