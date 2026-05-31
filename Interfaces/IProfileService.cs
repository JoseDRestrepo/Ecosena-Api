using EcoSENA.Api.Entities;
using EcoSENA.Api.Models.Profile;

namespace EcoSENA.Api.Interfaces
{
    public interface IProfileService
    {
        public Task<ProfileResDto> GetProfileAsync(int? id);
        public Task<Usuario> UpdateProfileAsync(int? id, EditProfileReqDto req);
    }
}
