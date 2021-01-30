using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using JwtAuthenticationSample.Models;

namespace JwtAuthenticationSample.Services
{
    public interface ITokenGenerator
    {
        Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user);
    }
}