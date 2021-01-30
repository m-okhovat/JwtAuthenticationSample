using JwtAuthenticationSample.Contexts;
using JwtAuthenticationSample.Models;

namespace JwtAuthenticationSample.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }


        public ApplicationUser GetById(string id)
        {
            return _context.Users.Find(id);
        }

    }
}