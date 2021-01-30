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
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserManagementService _userManagementService;
        private readonly IRefreshTokenService _tokenService;

        public UsersController(IUserService userService,
            IUserManagementService userManagementService,
            IAuthenticationService authenticationService,
            IRefreshTokenService tokenService)
        {
            _userService = userService;
            _userManagementService = userManagementService;
            _authenticationService = authenticationService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var result = await _userManagementService.RegisterAsync(model);
            return Ok(result);
        }

        [HttpPost("token")]
        public async Task<IActionResult> AuthenticateByUserPass(AuthenticationRequestModel model)
        {
            var authenticationResult = await _authenticationService.GenerateJwtByUserPass(model);
            Response.SetRefreshToken(authenticationResult.RefreshToken);
            return Ok(authenticationResult);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> AuthenticateByRefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var authenticationResultModel = await _authenticationService.GenerateJwtByRefreshToken(refreshToken);
            if (!authenticationResultModel.IsRefreshTokenNull)
                Response.SetRefreshToken(authenticationResultModel.RefreshToken);
            return Ok(authenticationResultModel);
        }

        [HttpPost("addRole")]
        public async Task<IActionResult> AddRoleAsync(AddRoleModel model)
        {
            var result = await _userManagementService.AddRoleAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("tokens/{id}")]
        public IActionResult GetRefreshTokens(string id)
        {
            var user = _userService.GetById(id);
            return Ok(user.RefreshTokens);
        }

        [Authorize]
        [HttpPatch("revoke-token")]
        public IActionResult RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });
            var response = _tokenService.RevokeToken(token);
            if (!response)
                return NotFound(new { message = "Token not found" });
            return Ok(new { message = "Token revoked" });
        }
    }

}