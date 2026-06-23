using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EcoSENA.Api.Data;
using EcoSENA.Api.Entities;
using EcoSENA.Api.Interfaces;
using EcoSENA.Api.Models.Reports;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

        public async Task<StatsReportDto> GetStats()
        {
            var fechaActual = DateTime.UtcNow;
            var primerDia = new DateTime(fechaActual.Year, fechaActual.Month, 1);
            var FinDeMes = primerDia.AddMonths(1);

            var totalReportesMes = await context.Reportes
                .Where(r => r.FechaEmision >= primerDia && r.FechaEmision <= FinDeMes)
                .CountAsync();

            var totalPendientes = await context.Reportes
                .Where(r => r.Estado == EstadoReporte.Pendiente)
                .CountAsync();

            var totalEnProgreso = await context.Reportes
                .Where(r => r.Estado == EstadoReporte.EnProgreso)
                .CountAsync();

            var totalResueltosMes = await context.Reportes
                .Where(r => r.FechaSolucion >= primerDia && r.FechaSolucion <= FinDeMes)
                .CountAsync();

            return new StatsReportDto
            {
                ReportesHechosMes = totalReportesMes,
                ReportesPendientes = totalPendientes,
                ReportesEnProgreso = totalEnProgreso,
                ReportesResueltosMes = totalResueltosMes
            };
        }

        public async Task<byte[]> ExportarExcelMesActualAsync()
        {
            var fechaActual = DateTime.UtcNow;
            var primerDia = new DateTime(fechaActual.Year, fechaActual.Month, 1);
            var FinDeMes = primerDia.AddMonths(1);

            var estadisticas = await context.Reportes
                .Where(r => r.FechaEmision >= primerDia && r.FechaEmision < FinDeMes)
                .GroupBy(r => r.Estado)
                .Select(e => new
                {
                    Estado = e.Key,
                    Cantidad = e.Count()
                })
                .AsNoTracking()
                .ToDictionaryAsync(x=> x.Estado, x => x.Cantidad);

            int Stat(EstadoReporte estado) => estadisticas.GetValueOrDefault(estado, 0);

            var pendientes = Stat(EstadoReporte.Pendiente);
            var enProgreso = Stat(EstadoReporte.EnProgreso);
            var resueltos = Stat(EstadoReporte.Resuelto);
            var total = estadisticas.Values.Sum();

            double Porcentaje(int dato) =>
                dato > 0 ? Math.Round((double)dato * 100 / total, 2) : 0;

            var porcentajePendientes = Porcentaje(pendientes);
            var porcentajeEnProgreso = Porcentaje(enProgreso);
            var porcentajeResueltos = Porcentaje(resueltos);

            var reportes = await context.Reportes
                .Include(r => r.Aprendiz)
                .Where(r => r.FechaEmision >= primerDia && r.FechaEmision < FinDeMes)
                .Select(r => new ReportExcelDto
                {
                    Id = r.Id,
                    Titulo = r.Titulo,
                    Descripcion = r.Descripcion,
                    Ubicacion = r.Ubicacion,
                    Estado = r.Estado.ToString(),
                    FechaPublicacion = r.FechaEmision,
                    FechaRevision = r.FechaRevision,
                    FechaSolucion = r.FechaSolucion,
                    Aprendiz = $"{r.Aprendiz.Nombre}, {r.Aprendiz.Apellido}"
                })
                .AsNoTracking()
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var hojaEstadisticas = workbook.Worksheets.Add("Estadisticas");

            var titulo = hojaEstadisticas.Cell(1, 1);
            titulo.Value = $"Estadísticas - {fechaActual:MMMM yyyy}";
            titulo.Style.Font.Bold = true;
            titulo.Style.Font.FontSize = 14;
            hojaEstadisticas.Range("A1:C1").Merge();

            hojaEstadisticas.Cell(3, 1).Value = "Estado";
            hojaEstadisticas.Cell(3, 2).Value = "Cantidad";
            hojaEstadisticas.Cell(3, 3).Value = "Porcentaje";
            foreach (var cell in hojaEstadisticas.Range("A3:C3").Cells())
            {
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#31AD44");
                cell.Style.Font.FontColor = XLColor.White;
            }

            var stats = new[]
            {
                ("Pendientes", pendientes, porcentajePendientes),
                ("En Progreso", enProgreso, porcentajeEnProgreso),
                ("Resueltos", resueltos, porcentajeResueltos),
                ("Total", total, 100)
            };

            for (int i = 0; i < stats.Length; i++)
            {
                hojaEstadisticas.Cell(i + 4, 1).Value = stats[i].Item1;
                hojaEstadisticas.Cell(i + 4, 2).Value = stats[i].Item2;
                hojaEstadisticas.Cell(i + 4, 3).Value =stats[i].Item3;
            }

            hojaEstadisticas.Columns().AdjustToContents();

            var hojaLista = workbook.Worksheets.Add("Reportes");

            var headers = new[]
            {
                "ID", "Titulo", "Descripción", "Ubicación", "Estado", "Fecha Publicación", "Fecha Revisión", "Fecha Solución", "Aprendiz"
            };

            for (int i=0; i < headers.Length; i++)
            {
                var cell = hojaLista.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#31AD44");
                cell.Style.Font.FontColor = XLColor.White;
            }

            for (int i = 0; i< reportes.Count; i++)
            {
                var reporte = reportes[i];
                var row = i + 2;
                hojaLista.Cell(row, 1).Value = reporte.Id;
                hojaLista.Cell(row, 2).Value = reporte.Titulo;
                hojaLista.Cell(row, 3).Value = reporte.Descripcion;
                hojaLista.Cell(row, 4).Value = reporte.Ubicacion;
                hojaLista.Cell(row, 5).Value = reporte.Estado;
                hojaLista.Cell(row, 6).Value = reporte.FechaPublicacion.ToString("yyyy-MM-dd HH:mm");
                hojaLista.Cell(row, 7).Value = reporte.FechaRevision?.ToString("yyyy-MM-dd HH:mm") ?? "-";
                hojaLista.Cell(row, 8).Value = reporte.FechaSolucion?.ToString("yyyy-MM-dd HH:mm") ?? "-";
                hojaLista.Cell(row, 9).Value = reporte.Aprendiz;
            }

            hojaLista.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
