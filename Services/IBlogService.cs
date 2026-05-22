using EcoSENA.Api.Entities;
using EcoSENA.Api.Models;

namespace EcoSENA.Api.Services
{
    public interface IBlogService
    {
        public Task<List<BlogListResDto>> GetEntradasAsync();
        public Task<EntradaResDto> GetEntradaAsync(int id);
        public Task<EntradaResDto> PostEntradaAsync(PostEntradaReqDto req, int redactorId);
        public Task<bool> DeleteEntradaAsync(int id);
        public Task<bool> UpdateEntradaAsync(int id, EditEntradaReqDto req);
    }
}
