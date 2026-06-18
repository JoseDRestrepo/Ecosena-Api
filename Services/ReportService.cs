using EcoSENA.Api.Data;
using EcoSENA.Api.Entities;
using EcoSENA.Api.Interfaces;
using EcoSENA.Api.Models.Reports;
using Microsoft.EntityFrameworkCore;

namespace EcoSENA.Api.Services
{
    public class ReportService(EcosenaDbContext context, ICloudinaryService cloudinary) : IReportService
    {
        public async Task<List<ReportListResDto>> GetReportsAsync()
        {
            var reportes = await context.Reportes
                .OrderByDescending(r=> r.FechaEmision)
                .Select(r => new ReportListResDto
                {
                    Id = r.Id,
                    Estado = r.Estado,
                    Ubicacion = r.Ubicacion,
                    FechaEmision = r.FechaEmision
                }).ToListAsync();

            return reportes;
        }

        public async Task<List<ReportListResDto>> GetReportsAprendizAsync(int id)
        {
            var reportes = await context.Reportes
                .OrderByDescending(r => r.FechaEmision)
                .Where(r => r.AprendizId == id)
                .Select(r => new ReportListResDto
                {
                    Id = r.Id,
                    Estado = r.Estado,
                    Ubicacion = r.Ubicacion,
                    FechaEmision = r.FechaEmision
                }).ToListAsync();

            return reportes;
        }

        public async Task<ReportResDto> GetReportAsync(int id)
        {
            var reporte = await context.Reportes.Include(r => r.Aprendiz).FirstOrDefaultAsync(r => r.Id == id);

            if (reporte == null)
            {
                return null;
            }

            return new ReportResDto
            {
                Id = reporte.Id,
                Titulo = reporte.Titulo,
                Descripcion = reporte.Descripcion,
                Estado = reporte.Estado,
                EmisorReporte = $"{reporte.Aprendiz.Nombre} {reporte.Aprendiz.Apellido}"
            };
        }

        public async Task<ReportResDto> PostReportAsync(ReportReqDto req, int redactorId)
        {
            var foto = await cloudinary.UploadImageAsync(req.Foto, "ecosena_reports");

            if (string.IsNullOrEmpty(foto))
            {
                throw new ArgumentException("La imagen es requerida para el reporte");
            }
            
            var reporte = new Reporte
            {
                Titulo = req.Titulo,
                Descripcion = req.Descripcion,
                Estado = EstadoReporte.Pendiente,
                Ubicacion = req.Ubicacion,
                Foto = foto,
                FechaEmision = DateTime.UtcNow,
                AprendizId = redactorId
            };

            context.Add(reporte);
            await context.SaveChangesAsync();

            await context.Entry(reporte).Reference(r => r.Aprendiz).LoadAsync();

            return new ReportResDto
            {
                Id = reporte.Id,
                Titulo = reporte.Titulo,
                Estado = reporte.Estado,
                Descripcion = reporte.Descripcion,
                EmisorReporte = $"{reporte.Aprendiz.Nombre} {reporte.Aprendiz.Apellido}",
                Foto = reporte.Foto
            };
        }

        public async Task<bool> UpdateReportAsync(int id)
        {
            var reporte = await context.Reportes.FindAsync(id);
            if (reporte == null)
            {
                return false;
            }

            reporte.Estado = reporte.Estado switch
            {
                EstadoReporte.Pendiente => EstadoReporte.EnProgreso,
                EstadoReporte.EnProgreso => EstadoReporte.Resuelto,
                EstadoReporte.Resuelto => EstadoReporte.Resuelto,
                _ => reporte.Estado
            };

            if (reporte.Estado == EstadoReporte.EnProgreso)
            {
                reporte.FechaRevision = DateTime.UtcNow;
            } else if (reporte.Estado == EstadoReporte.Resuelto)
            {
                reporte.FechaSolucion = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();
            return true;
        }
    }
}
