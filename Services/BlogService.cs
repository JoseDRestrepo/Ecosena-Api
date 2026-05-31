using EcoSENA.Api.Data;
using EcoSENA.Api.Entities;
using EcoSENA.Api.Interfaces;
using EcoSENA.Api.Models.Blog;
using Microsoft.EntityFrameworkCore;

namespace EcoSENA.Api.Services
{
    public class BlogService(EcosenaDbContext context, ICloudinaryService cloudinary) : IBlogService
    {
        public async Task<bool> DeleteEntradaAsync(int id)
        {
            var entrada = await context.Entradas.FindAsync(id);

            if (entrada == null)
            {
                return false;
            }

            context.Remove(entrada);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<EntradaResDto> GetEntradaAsync(int id)
        {
            var entrada = await context.Entradas
                .Include(e => e.Redactor)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entrada == null)
            {
                return null;
            }

            return new EntradaResDto
            {
                Id = id,
                Titulo = entrada.Titulo,
                Contenido = entrada.Contenido,
                FechaPublicacion = entrada.FechaPublicacion,
                NombreRedactor = $"{entrada.Redactor.Nombre} {entrada.Redactor.Apellido}",
                Portada = entrada.Portada
            };
        }

        public async Task<List<BlogListResDto>> GetEntradasAsync()
        {
            var entradas = await context.Entradas.Include(e => e.Redactor)
                .OrderByDescending(e => e.FechaPublicacion)
                .Select(e => new BlogListResDto
                {
                    Id = e.Id,
                    Titulo = e.Titulo,
                    FechaPublicacion= e.FechaPublicacion,
                    NombreRedactor = $"{e.Redactor.Nombre} {e.Redactor.Apellido}",
                    Portada= e.Portada
                }).ToListAsync();

            return entradas;
        }

        public async Task<EntradaResDto> PostEntradaAsync(PostEntradaReqDto req, int redactorId)
        {
            string portada = await GetPortada(req.Portada);

            var entrada = new Entrada
            {
                Titulo = req.Titulo,
                Contenido = req.Contenido,
                Portada = portada,
                RedactorId = redactorId,
                FechaPublicacion = DateTime.UtcNow
            };

            context.Add(entrada);
            await context.SaveChangesAsync();

            await context.Entry(entrada).Reference(e => e.Redactor).LoadAsync();

            var entradaRes = new EntradaResDto
            {
                Id = entrada.Id,
                Titulo = entrada.Titulo,
                Contenido = entrada.Contenido,
                Portada = entrada.Portada,
                FechaPublicacion = entrada.FechaPublicacion,
                NombreRedactor = $"{entrada.Redactor.Nombre} {entrada.Redactor.Apellido}"
            };

            return entradaRes;
        }

        private async Task<string> GetPortada(IFormFile? req)
        {
            string portada = await cloudinary.UploadImageAsync(req, "ecosena_blog");

            if (string.IsNullOrEmpty(portada))
            {
                portada = Environment.GetEnvironmentVariable("DEFAULT_IMAGE_BLOG");
            }

            return portada;
        }

        public async Task<bool> UpdateEntradaAsync(int id, EditEntradaReqDto req)
        {
            var portada = await GetPortada(req.Portada);

            var entrada = await context.Entradas.FindAsync(id);

            if (entrada == null)
            {
                return false;
            }

            entrada.Titulo = req.Titulo;
            entrada.Contenido = req.Contenido;
            entrada.Portada = portada;
            
            context.Update(entrada);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
