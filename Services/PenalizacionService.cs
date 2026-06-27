using EcoSENA.Api.Data;
using EcoSENA.Api.Entities;
using EcoSENA.Api.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcoSENA.Api.Services
{
    public class PenalizacionService : IPenalizacionService
    {
        private readonly EcosenaDbContext _context;

        public PenalizacionService(EcosenaDbContext context)
        {
            _context = context;
        }

        public async Task PenalizacionAdmin(int id, int reporteId)
        {
            var reporte = await _context.Reportes.FindAsync(reporteId);

            _context.Infracciones.Add(new Infraccion
            {
                AprendizId = reporte.AprendizId,
                Tipo = TipoInfraccion.PenalizacionAdmin,
                FechaInfraccion = DateTime.UtcNow,
                FechaExpiracion = null,
                ReporteId = reporteId
            });

            _context.Reportes.Remove(reporte);

            await AplicarRolPenalizadoAsync(reporte.AprendizId);

            await _context.SaveChangesAsync();
        }

        public async Task RegistrarIntento(int aprendizId)
        {
            var totalInfracciones = await _context.Infracciones
            .CountAsync(i => i.AprendizId == aprendizId);

            var nuevaFechaExpiracion = totalInfracciones switch
            {
                0 => DateTime.UtcNow.AddDays(7),
                1 => DateTime.UtcNow.AddMonths(1),
                _ => (DateTime?)null               
            };

            _context.Infracciones.Add(new Infraccion
            {
                AprendizId = aprendizId,
                Tipo = TipoInfraccion.Intento,
                FechaInfraccion = DateTime.UtcNow,
                FechaExpiracion = nuevaFechaExpiracion
            });

            if (nuevaFechaExpiracion is null)
                await AplicarRolPenalizadoAsync(aprendizId);
        }

        public async Task<(bool Bloq, DateTime? Expiracion)> VerificarPenalizacion(int aprendizId)
        {
            var ultimaInfraccion = await _context.Infracciones
            .Where(i => i.AprendizId == aprendizId)
            .OrderByDescending(i => i.FechaInfraccion)
            .FirstOrDefaultAsync();

            if (ultimaInfraccion is null) return (false, null);

            if (ultimaInfraccion.FechaExpiracion is null) return (true, null);

            if (ultimaInfraccion.FechaExpiracion > DateTime.UtcNow)
                return (true, ultimaInfraccion.FechaExpiracion);

            return (false, null);
        }

        private async Task AplicarRolPenalizadoAsync(int aprendizId)
        {
            var usuario = await _context.Usuarios.FindAsync(aprendizId);

            usuario.Rol = RolUsuario.Penalizado;

            await _context.SaveChangesAsync();
        }
    }
}
