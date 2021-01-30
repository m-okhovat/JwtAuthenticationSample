using System;
using System.Security.Cryptography;
using JwtAuthenticationSample.Models;

namespace JwtAuthenticationSample.Services
{
    public static class RefreshTokenGenerator
    {
        public static RefreshToken CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var generator = new RNGCryptoServiceProvider())
            {
                generator.GetBytes(randomNumber);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    Expires = DateTime.UtcNow.AddDays(10),
                    Created = DateTime.UtcNow
                };
            }
        }
    }
}