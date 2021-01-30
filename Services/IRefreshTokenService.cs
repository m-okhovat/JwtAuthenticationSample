namespace JwtAuthenticationSample.Services
{
    public interface IRefreshTokenService
    {
        bool RevokeToken(string token);
    }
}