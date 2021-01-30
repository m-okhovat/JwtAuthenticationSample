using JwtAuthenticationSample.Models;
using Microsoft.AspNetCore.Http;

namespace JwtAuthenticationSample.Controllers
{
    public static class HttpResponseExtensions
    {
        public static void SetRefreshToken(this HttpResponse response, RefreshToken token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = token.Expires,
            };
            response.Cookies.Append("refreshToken", token.Token, cookieOptions);
        }
    }
}