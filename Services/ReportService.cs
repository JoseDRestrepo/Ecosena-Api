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
                .Include(r=> r.Ambiente)
                .OrderByDescending(r=> r.FechaEmision)
                .Select(r => new ReportListResDto
                {
                    Id = r.Id,
                    Estado = r.Estado,
                    Ubicacion = r.Ambiente.Nombre,
                    FechaEmision = r.FechaEmision
                }).ToListAsync();

            return reportes;
        }

        public async Task<List<ReportListResDto>> GetReportsAprendizAsync(int id)
        {
            var reportes = await context.Reportes.Include(r=> r.Ambiente)
                .OrderByDescending(r => r.FechaEmision)
                .Where(r => r.AprendizId == id)
                .Select(r => new ReportListResDto
                {
                    Id = r.Id,
                    Estado = r.Estado,
                    Ubicacion = r.Ambiente.Nombre,
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

        public async Task<IEnumerable<Ambiente>> GetListaAmbientes()
        {
            var listaAmbientes = await context.Ambientes.ToListAsync();

            return listaAmbientes;
        }

        public async Task<bool> ReporteActivo(int id)
        {
            var estadoReporte = await context.Ambientes.Where(a=> a.Id == id)
                .Select(a=> a.ReporteActivo).FirstOrDefaultAsync();

            return estadoReporte;
        }

        public async Task<ReportResDto> PostReportAsync(ReportReqDto req, int redactorId)
        {
            var foto = await cloudinary.UploadImageAsync(req.Foto, "ecosena_reports");

            if (string.IsNullOrEmpty(foto))
            {
                throw new ArgumentException("La imagen es requerida para el reporte");
            }

            var ambiente = await context.Ambientes.FindAsync(req.IdAmbiente);
            
            var reporte = new Reporte
            {
                Titulo = req.Titulo,
                Descripcion = req.Descripcion,
                Estado = EstadoReporte.Pendiente,
                AmbienteId = req.IdAmbiente,
                Foto = foto,
                FechaEmision = DateTime.UtcNow,
                AprendizId = redactorId
            };

            context.Add(reporte);
            ambiente.ReporteActivo = true;
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

            var ambiente = await context.Ambientes.FindAsync(reporte.AmbienteId);

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
                ambiente.ReporteActivo = false;
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

            // === OBTENER ESTADÍSTICAS DEL MES ===
            var estadisticasMes = await context.Reportes
                .Where(r => r.FechaEmision >= primerDia && r.FechaEmision < FinDeMes)
                .GroupBy(r => r.Estado)
                .Select(e => new
                {
                    Estado = e.Key,
                    Cantidad = e.Count()
                })
                .AsNoTracking()
                .ToDictionaryAsync(x => x.Estado, x => x.Cantidad);

            int StatMes(EstadoReporte estado) => estadisticasMes.GetValueOrDefault(estado, 0);

            var pendientesMes = StatMes(EstadoReporte.Pendiente);
            var enProgresoMes = StatMes(EstadoReporte.EnProgreso);
            var resueltosMes = StatMes(EstadoReporte.Resuelto);
            var totalMes = estadisticasMes.Values.Sum();

            double PorcentajeMes(int dato) =>
                dato > 0 ? Math.Round((double)dato * 100 / totalMes, 2) : 0;

            var porcentajePendientesMes = PorcentajeMes(pendientesMes);
            var porcentajeEnProgresoMes = PorcentajeMes(enProgresoMes);
            var porcentajeResueltosMes = PorcentajeMes(resueltosMes);

            // === OBTENER ESTADÍSTICAS GLOBALES (TODOS LOS MESES) ===
            var estadisticasGlobales = await context.Reportes
                .GroupBy(r => r.Estado)
                .Select(e => new
                {
                    Estado = e.Key,
                    Cantidad = e.Count()
                })
                .AsNoTracking()
                .ToDictionaryAsync(x => x.Estado, x => x.Cantidad);

            int StatGlobal(EstadoReporte estado) => estadisticasGlobales.GetValueOrDefault(estado, 0);

            var pendientesGlobal = StatGlobal(EstadoReporte.Pendiente);
            var enProgresoGlobal = StatGlobal(EstadoReporte.EnProgreso);
            var resueltosGlobal = StatGlobal(EstadoReporte.Resuelto);
            var totalGlobal = estadisticasGlobales.Values.Sum();

            double PorcentajeGlobal(int dato) =>
                dato > 0 ? Math.Round((double)dato * 100 / totalGlobal, 2) : 0;

            var porcentajePendientesGlobal = PorcentajeGlobal(pendientesGlobal);
            var porcentajeEnProgresoGlobal = PorcentajeGlobal(enProgresoGlobal);
            var porcentajeResueltosGlobal = PorcentajeGlobal(resueltosGlobal);

            // === OBTENER LISTA DE REPORTES DEL MES ===
            var reportes = await context.Reportes.Include(r=> r.Ambiente)
                .Include(r => r.Aprendiz)
                .Where(r => r.FechaEmision >= primerDia && r.FechaEmision < FinDeMes)
                .Select(r => new ReportExcelDto
                {
                    Id = r.Id,
                    Titulo = r.Titulo,
                    Descripcion = r.Descripcion,
                    Ubicacion = r.Ambiente.Nombre,
                    Estado = r.Estado.ToString(),
                    FechaPublicacion = r.FechaEmision,
                    FechaRevision = r.FechaRevision,
                    FechaSolucion = r.FechaSolucion,
                    Aprendiz = $"{r.Aprendiz.Nombre} {r.Aprendiz.Apellido}"
                })
                .AsNoTracking()
                .ToListAsync();

            // === CREAR EXCEL CON EPPLUS ===
            ExcelPackage.License.SetNonCommercialPersonal("EcoSENA");
            using var package = new ExcelPackage();

            // ============================================================
            // HOJA 1: ESTADÍSTICAS CON TABLAS Y GRÁFICOS
            // ============================================================
            var hojaEstadisticas = package.Workbook.Worksheets.Add("Estadisticas");

            // --- TÍTULO ---
            hojaEstadisticas.Cells[1, 1].Value = $"Estadísticas - {fechaActual:MMMM yyyy}";
            hojaEstadisticas.Cells[1, 1, 1, 3].Merge = true;
            hojaEstadisticas.Cells[1, 1].Style.Font.Size = 14;
            hojaEstadisticas.Cells[1, 1].Style.Font.Bold = true;

            // ============================================================
            // SECCIÓN 1: TABLA DEL MES (Columnas A-C, Filas 3-7)
            // ============================================================
            hojaEstadisticas.Cells[3, 1].Value = "Estado";
            hojaEstadisticas.Cells[3, 2].Value = "Cantidad (Mes)";
            hojaEstadisticas.Cells[3, 3].Value = "Porcentaje (Mes)";

            using (var range = hojaEstadisticas.Cells[3, 1, 3, 3])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(49, 173, 68));
                range.Style.Font.Color.SetColor(Color.White);
            }

            var statsMes = new[]
            {
                ("Pendientes", pendientesMes, porcentajePendientesMes),
                ("En Progreso", enProgresoMes, porcentajeEnProgresoMes),
                ("Resueltos", resueltosMes, porcentajeResueltosMes),
                ("Total", totalMes, porcentajePendientesMes + porcentajeEnProgresoMes + porcentajeResueltosMes)
            };

            for (int i = 0; i < statsMes.Length; i++)
            {
                hojaEstadisticas.Cells[i + 4, 1].Value = statsMes[i].Item1;
                hojaEstadisticas.Cells[i + 4, 2].Value = statsMes[i].Item2;
                hojaEstadisticas.Cells[i + 4, 3].Value = statsMes[i].Item3;
            }

            var tablaRangoMes = hojaEstadisticas.Cells[3, 1, 7, 3];
            var tablaEstadisticasMes = hojaEstadisticas.Tables.Add(tablaRangoMes, "TablaEstadisticasMes");
            tablaEstadisticasMes.TableStyle = OfficeOpenXml.Table.TableStyles.Medium9;
            tablaEstadisticasMes.ShowFilter = true;

            // --- 📊 GRÁFICO 1: PASTEL DEL MES (Columna E, Filas 3-7) ---
            var pieChartMes = hojaEstadisticas.Drawings.AddChart("PieChartMes", eChartType.Pie);
            pieChartMes.SetPosition(3, 0, 4, 0);
            pieChartMes.SetSize(350, 220);

            var seriePieMes = pieChartMes.Series.Add(
                hojaEstadisticas.Cells[4, 3, 6, 3],
                hojaEstadisticas.Cells[4, 1, 6, 1]
            );
            seriePieMes.Header = "Porcentaje por Estado (Mes)";

            pieChartMes.Title.Text = $"Distribución - {fechaActual:MMMM yyyy}";
            pieChartMes.Title.Font.Size = 11;
            pieChartMes.Title.Font.Bold = true;

            // ============================================================
            // SECCIÓN 2: TABLA GLOBAL (Columnas A-C, Filas 12-16) - MOVIDA 5 FILAS MÁS ABAJO
            // ============================================================
            hojaEstadisticas.Cells[12, 1].Value = "Estado";
            hojaEstadisticas.Cells[12, 2].Value = "Cantidad (Global)";
            hojaEstadisticas.Cells[12, 3].Value = "Porcentaje (Global)";

            using (var rangeGlobal = hojaEstadisticas.Cells[12, 1, 12, 3])
            {
                rangeGlobal.Style.Font.Bold = true;
                rangeGlobal.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeGlobal.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 102, 204));
                rangeGlobal.Style.Font.Color.SetColor(Color.White);
            }

            var statsGlobal = new[]
            {
                ("Pendientes", pendientesGlobal, porcentajePendientesGlobal),
                ("En Progreso", enProgresoGlobal, porcentajeEnProgresoGlobal),
                ("Resueltos", resueltosGlobal, porcentajeResueltosGlobal),
                ("Total", totalGlobal, porcentajePendientesGlobal + porcentajeEnProgresoGlobal + porcentajeResueltosGlobal)
            };

            for (int i = 0; i < statsGlobal.Length; i++)
            {
                hojaEstadisticas.Cells[i + 13, 1].Value = statsGlobal[i].Item1;
                hojaEstadisticas.Cells[i + 13, 2].Value = statsGlobal[i].Item2;
                hojaEstadisticas.Cells[i + 13, 3].Value = statsGlobal[i].Item3;
            }

            var tablaRangoGlobal = hojaEstadisticas.Cells[12, 1, 16, 3];
            var tablaEstadisticasGlobal = hojaEstadisticas.Tables.Add(tablaRangoGlobal, "TablaEstadisticasGlobal");
            tablaEstadisticasGlobal.TableStyle = OfficeOpenXml.Table.TableStyles.Medium9;
            tablaEstadisticasGlobal.ShowFilter = true;

            // --- 📊 GRÁFICO 2: PASTEL GLOBAL (Columna I, Filas 12-16) - MOVIDO CON SU TABLA ---
            var pieChartGlobal = hojaEstadisticas.Drawings.AddChart("PieChartGlobal", eChartType.Pie);
            pieChartGlobal.SetPosition(3, 0, 12, 60);
            pieChartGlobal.SetSize(350, 220);

            var seriePieGlobal = pieChartGlobal.Series.Add(
                hojaEstadisticas.Cells[13, 3, 15, 3],
                hojaEstadisticas.Cells[13, 1, 15, 1]
            );
            seriePieGlobal.Header = "Porcentaje por Estado (Global)";

            pieChartGlobal.Title.Text = "Distribución Global";
            pieChartGlobal.Title.Font.Size = 11;
            pieChartGlobal.Title.Font.Bold = true;

            // ============================================================
            // SECCIÓN 3: TABLA COMPARATIVA (Columnas E-G, Filas 23-27) - MOVIDA MÁS ABAJO
            // ============================================================
            hojaEstadisticas.Cells[23, 5].Value = "Comparativa de Porcentajes";
            hojaEstadisticas.Cells[23, 5, 23, 7].Merge = true;
            hojaEstadisticas.Cells[23, 5].Style.Font.Size = 14;
            hojaEstadisticas.Cells[23, 5].Style.Font.Bold = true;

            hojaEstadisticas.Cells[24, 5].Value = "Estado";
            hojaEstadisticas.Cells[24, 6].Value = "Mes %";
            hojaEstadisticas.Cells[24, 7].Value = "Global %";

            using (var rangeComparativa = hojaEstadisticas.Cells[24, 5, 24, 7])
            {
                rangeComparativa.Style.Font.Bold = true;
                rangeComparativa.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeComparativa.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 128, 0));
                rangeComparativa.Style.Font.Color.SetColor(Color.White);
            }

            var comparativa = new[]
            {
                ("Pendientes", porcentajePendientesMes, porcentajePendientesGlobal),
                ("En Progreso", porcentajeEnProgresoMes, porcentajeEnProgresoGlobal),
                ("Resueltos", porcentajeResueltosMes, porcentajeResueltosGlobal)
            };

            for (int i = 0; i < comparativa.Length; i++)
            {
                hojaEstadisticas.Cells[i + 25, 5].Value = comparativa[i].Item1;
                hojaEstadisticas.Cells[i + 25, 6].Value = comparativa[i].Item2;
                hojaEstadisticas.Cells[i + 25, 7].Value = comparativa[i].Item3;
            }

            // --- 📊 GRÁFICO 3: BARRAS COMPARATIVO (Columna I, Filas 23-27) - MOVIDO CON SU TABLA ---
            var barChartComparativo = hojaEstadisticas.Drawings.AddChart("BarChartComparativo", eChartType.ColumnClustered);
            barChartComparativo.SetPosition(23, 0, 9, 0);
            barChartComparativo.SetSize(450, 250);

            var serieMes = barChartComparativo.Series.Add(
                hojaEstadisticas.Cells[25, 6, 27, 6],
                hojaEstadisticas.Cells[25, 5, 27, 5]
            );
            serieMes.Header = "Mes %";

            var serieGlobal = barChartComparativo.Series.Add(
                hojaEstadisticas.Cells[25, 7, 27, 7],
                hojaEstadisticas.Cells[25, 5, 27, 5]
            );
            serieGlobal.Header = "Global %";

            barChartComparativo.Title.Text = "Comparativa: Mes vs Global";
            barChartComparativo.Title.Font.Size = 11;
            barChartComparativo.Title.Font.Bold = true;

            barChartComparativo.YAxis.Title.Text = "Porcentaje (%)";
            barChartComparativo.YAxis.Title.Font.Size = 9;
            barChartComparativo.YAxis.Title.Font.Bold = true;

            // --- Ajustar columnas ---
            hojaEstadisticas.Cells[hojaEstadisticas.Dimension.Address].AutoFitColumns();

            // ============================================================
            // HOJA 2: LISTA DE REPORTES DEL MES
            // ============================================================
            var hojaLista = package.Workbook.Worksheets.Add("Reportes");

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

            if (reportes.Count > 0)
            {
                var ultimaFila = reportes.Count + 1;
                var rangoReportes = hojaLista.Cells[1, 1, ultimaFila, headers.Length];
                var tablaReportes = hojaLista.Tables.Add(rangoReportes, "TablaReportes");
                tablaReportes.TableStyle = OfficeOpenXml.Table.TableStyles.Medium9;
                tablaReportes.ShowFilter = true;
            }

            hojaLista.Cells[hojaLista.Dimension.Address].AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }
    }
}
