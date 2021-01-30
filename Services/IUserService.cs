﻿using System.Threading.Tasks;
using JwtAuthenticationSample.Models;

namespace JwtAuthenticationSample.Services
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterModel model);
        Task<AuthenticationResultModel> GenerateAuthenticationToken(AuthenticationRequestModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
    }
}