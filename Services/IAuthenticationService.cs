using System.Threading.Tasks;
using JwtAuthenticationSample.Models;

namespace JwtAuthenticationSample.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResultModel> GenerateJwtByUserPass(AuthenticationRequestModel model);
        Task<AuthenticationResultModel> GenerateJwtByRefreshToken(string token);
    }
}