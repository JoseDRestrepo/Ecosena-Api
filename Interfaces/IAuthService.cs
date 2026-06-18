using EcoSENA.Api.Entities;
using EcoSENA.Api.Models.Auth;

namespace EcoSENA.Api.Interfaces
{
    public interface IAuthService
    {
        public Task<Usuario?> RegisterAsync(RegisterReqDto req);
        public Task<LoginResDto?> LoginAsync(LoginReqDto req);
    }
}
