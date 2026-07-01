using EcoSENA.Api.Entities;
using EcoSENA.Api.Models.Blog;

namespace EcoSENA.Api.Interfaces
{
    public interface IBlogService
    {
        public Task<List<BlogListResDto>> GetEntradasAsync(string? busqueda = null);
        public Task<EntradaResDto> GetEntradaAsync(int id);
        public Task<EntradaResDto> PostEntradaAsync(PostEntradaReqDto req, int redactorId);
        public Task<bool> DeleteEntradaAsync(int id);
        public Task<bool> UpdateEntradaAsync(int id, EditEntradaReqDto req);
    }
}
