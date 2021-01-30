using System.Collections.Generic;

namespace JwtAuthenticationSample.Models
{
    public class AuthenticationResultModel
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public bool IsRefreshTokenNull => RefreshToken == null || string.IsNullOrEmpty(RefreshToken.Token);
        public static AuthenticationResultModel Failed(string message)
        {
            return new AuthenticationResultModel()
            {
                IsAuthenticated = false,
                Message = message
            };
        }
    }
}