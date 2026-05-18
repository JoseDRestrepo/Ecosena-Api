using EcoSENA.Api.Entities;
using EcoSENA.Api.Models;

namespace EcoSENA.Api.Services
{
    public interface IProfileService
    {
        public Task<ProfileResDto> GetProfileAsync(int? id);
        public Task<Usuario> UpdateProfileAsync(int? id, EditProfileReqDto req);
    }
}
