using System.Threading.Tasks;
using JwtAuthenticationSample.Models;

namespace JwtAuthenticationSample.Services
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterModel model);
    }
}