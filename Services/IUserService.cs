using System.Threading.Tasks;
using JwtAuthenticationSample.Models;

namespace JwtAuthenticationSample.Services
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterModel model);
        Task<AuthenticationResultModel> GenerateJwtByUserPass(AuthenticationRequestModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
        Task<AuthenticationResultModel> GenerateJwtByRefreshToken(string token);
        ApplicationUser GetById(string id);
    }
}