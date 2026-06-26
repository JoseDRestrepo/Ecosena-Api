using EcoSENA.Api.Data;
using EcoSENA.Api.Entities;
using EcoSENA.Api.Interfaces;
using EcoSENA.Api.Models.Reports;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.Drawing;

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

            // === OBTENER ESTADÍSTICAS ===
            var estadisticas = await context.Reportes
                .Where(r => r.FechaEmision >= primerDia && r.FechaEmision < FinDeMes)
                .GroupBy(r => r.Estado)
                .Select(e => new
                {
                    Estado = e.Key,
                    Cantidad = e.Count()
                })
                .AsNoTracking()
                .ToDictionaryAsync(x => x.Estado, x => x.Cantidad);

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

            // === OBTENER LISTA DE REPORTES ===
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

            // === CREAR EXCEL CON EPPLUS ===
            // ✅ CORRECCIÓN: Usar la forma correcta de configurar la licencia en EPPlus 8
            ExcelPackage.License.SetNonCommercialPersonal("EcoSENA");
            using var package = new ExcelPackage();

            // ============================================================
            // HOJA 1: ESTADÍSTICAS
            // ============================================================
            var hojaEstadisticas = package.Workbook.Worksheets.Add("Estadisticas");

            // --- TÍTULO ---
            hojaEstadisticas.Cells[1, 1].Value = $"Estadísticas - {fechaActual:MMMM yyyy}";
            hojaEstadisticas.Cells[1, 1, 1, 3].Merge = true;
            hojaEstadisticas.Cells[1, 1].Style.Font.Size = 14;
            hojaEstadisticas.Cells[1, 1].Style.Font.Bold = true;

            // --- ENCABEZADOS ---
            hojaEstadisticas.Cells[3, 1].Value = "Estado";
            hojaEstadisticas.Cells[3, 2].Value = "Cantidad";
            hojaEstadisticas.Cells[3, 3].Value = "Porcentaje";

            using (var range = hojaEstadisticas.Cells[3, 1, 3, 3])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(49, 173, 68));
                range.Style.Font.Color.SetColor(Color.White);
            }

            // --- DATOS ---
            var stats = new[]
            {
                ("Pendientes", pendientes, porcentajePendientes),
                ("En Progreso", enProgreso, porcentajeEnProgreso),
                ("Resueltos", resueltos, porcentajeResueltos),
                ("Total", total, 100)
            };

            for (int i = 0; i < stats.Length; i++)
            {
                hojaEstadisticas.Cells[i + 4, 1].Value = stats[i].Item1;
                hojaEstadisticas.Cells[i + 4, 2].Value = stats[i].Item2;
                hojaEstadisticas.Cells[i + 4, 3].Value = stats[i].Item3;
            }

            // --- TABLA FORMAL ---
            var tablaRango = hojaEstadisticas.Cells[3, 1, 7, 3];
            var tablaEstadisticas = hojaEstadisticas.Tables.Add(tablaRango, "TablaEstadisticas");
            tablaEstadisticas.TableStyle = OfficeOpenXml.Table.TableStyles.Medium9;
            tablaEstadisticas.ShowFilter = true;

            // --- 📊 GRÁFICO 1: PASTEL (CORREGIDO) ---
            var pieChart = hojaEstadisticas.Drawings.AddChart("PieChart", eChartType.Pie);
            pieChart.SetPosition(1, 0, 5, 0);
            pieChart.SetSize(400, 280);

            var seriePie = pieChart.Series.Add(
                hojaEstadisticas.Cells[4, 3, 6, 3],
                hojaEstadisticas.Cells[4, 1, 6, 1]
            );
            seriePie.Header = "Porcentaje por Estado";

            pieChart.Title.Text = "Distribución de Reportes (%)";
            pieChart.Title.Font.Size = 14;
            pieChart.Title.Font.Bold = true;

            // --- 📊 GRÁFICO 2: BARRAS COMBINADO ---
            var barChart = hojaEstadisticas.Drawings.AddChart("BarChart", eChartType.ColumnClustered);
            barChart.SetPosition(18, 0, 5, 0);
            barChart.SetSize(500, 280);

            var serieCantidad = barChart.Series.Add(
                hojaEstadisticas.Cells[4, 2, 6, 2],
                hojaEstadisticas.Cells[4, 1, 6, 1]
            );
            serieCantidad.Header = "Cantidad";

            var seriePorcentajeBar = barChart.Series.Add(
                hojaEstadisticas.Cells[4, 3, 6, 3],
                hojaEstadisticas.Cells[4, 1, 6, 1]
            );
            seriePorcentajeBar.Header = "Porcentaje (%)";


            barChart.Title.Text = "Comparativa: Cantidad vs Porcentaje";
            barChart.Title.Font.Size = 14;
            barChart.Title.Font.Bold = true;

            // --- Ajustar columnas ---
            hojaEstadisticas.Cells[hojaEstadisticas.Dimension.Address].AutoFitColumns();

            // ============================================================
            // HOJA 2: LISTA DE REPORTES
            // ============================================================
            var hojaLista = package.Workbook.Worksheets.Add("Reportes");

            // --- ENCABEZADOS ---
            var headers = new[]
            {
                "ID", "Titulo", "Descripción", "Ubicación", "Estado",
                "Fecha Publicación", "Fecha Revisión", "Fecha Solución", "Aprendiz"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = hojaLista.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(49, 173, 68));
                cell.Style.Font.Color.SetColor(Color.White);
            }

            // --- DATOS ---
            for (int i = 0; i < reportes.Count; i++)
            {
                var reporte = reportes[i];
                var row = i + 2;
                hojaLista.Cells[row, 1].Value = reporte.Id;
                hojaLista.Cells[row, 2].Value = reporte.Titulo;
                hojaLista.Cells[row, 3].Value = reporte.Descripcion;
                hojaLista.Cells[row, 4].Value = reporte.Ubicacion;
                hojaLista.Cells[row, 5].Value = reporte.Estado;
                hojaLista.Cells[row, 6].Value = reporte.FechaPublicacion.ToString("yyyy-MM-dd HH:mm");
                hojaLista.Cells[row, 7].Value = reporte.FechaRevision?.ToString("yyyy-MM-dd HH:mm") ?? "-";
                hojaLista.Cells[row, 8].Value = reporte.FechaSolucion?.ToString("yyyy-MM-dd HH:mm") ?? "-";
                hojaLista.Cells[row, 9].Value = reporte.Aprendiz;
            }

            // --- TABLA FORMAL ---
            if (reportes.Count > 0)
            {
                var ultimaFila = reportes.Count + 1;
                var rangoReportes = hojaLista.Cells[1, 1, ultimaFila, headers.Length];
                var tablaReportes = hojaLista.Tables.Add(rangoReportes, "TablaReportes");
                tablaReportes.TableStyle = OfficeOpenXml.Table.TableStyles.Medium9;
                tablaReportes.ShowFilter = true;
            }

            hojaLista.Cells[hojaLista.Dimension.Address].AutoFitColumns();

            // === GUARDAR ===
            return await package.GetAsByteArrayAsync();
        }
    }
}
