using JwtAuthenticationSample.Models;
using JwtAuthenticationSample.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JwtAuthenticationSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var result = await _userService.RegisterAsync(model);
            return Ok(result);
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync(AuthenticationRequestModel model)
        {
            var authenticationResult = await _userService.GenerateJwtByUserPass(model);
            Response.SetRefreshToken(authenticationResult.RefreshToken);
            return Ok(authenticationResult);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var authenticationResultModel = await _userService.GenerateJwtByRefreshToken(refreshToken);
            if (!authenticationResultModel.IsRefreshTokenNull)
                Response.SetRefreshToken(authenticationResultModel.RefreshToken);
            return Ok(authenticationResultModel);
        }

        [HttpPost("addRole")]
        public async Task<IActionResult> AddRoleAsync(AddRoleModel model)
        {
            var result = await _userService.AddRoleAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("tokens/{id}")]
        public IActionResult GetRefreshTokens(string id)
        {
            var user = _userService.GetById(id);
            return Ok(user.RefreshTokens);
        }
    }
}