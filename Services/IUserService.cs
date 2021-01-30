using JwtAuthenticationSample.Models;

namespace JwtAuthenticationSample.Services
{
    public interface IUserService
    {
        ApplicationUser GetById(string id);
    }
}