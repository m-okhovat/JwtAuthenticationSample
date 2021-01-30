using System.Threading.Tasks;
using JwtAuthenticationSample.Models;

namespace JwtAuthenticationSample.Services
{
    public interface IUserManagementService
    {
        Task<string> AddRoleAsync(AddRoleModel model);
        Task<string> RegisterAsync(RegisterModel model);
    }
}