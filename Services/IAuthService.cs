using EcoSENA.Api.Entities;
using EcoSENA.Api.Models;

namespace EcoSENA.Api.Services
{
    public interface IAuthService
    {
        public Task<Usuario?> RegisterAsync(RegisterReqDto req);

        public Task<LoginResDto?> LoginAsync(LoginReqDto req);
    }
}
