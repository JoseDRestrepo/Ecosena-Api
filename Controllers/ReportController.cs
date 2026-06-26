using EcoSENA.Api.Interfaces;
using EcoSENA.Api.Models.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcoSENA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController(IReportService service, ICensorshipService censorship) : ControllerBase
    {
        [HttpGet("AllReports")]
        [Authorize(Roles ="Administrador")]
        public async Task<ActionResult<List<ReportListResDto>>> GetReports()
        {
            var reportes = await service.GetReportsAsync();
            if (reportes == null)
            {
                return NotFound();
            }

            return Ok(reportes);
        }

        [HttpGet("Reports")]
        public async Task<ActionResult<List<ReportListResDto>>> GetYourReports()
        {
            var id = GetUserIdFromToken();
            var reportes = await service.GetReportsAprendizAsync(id);
            if (reportes == null)
            {
                return NotFound();
            }

            return Ok(reportes);
        }

        [HttpGet("{id}")]
        [Authorize(Roles ="Administrador")]
        public async Task<ActionResult<ReportResDto>> GetReport(int id)
        {
            var reporte = await service.GetReportAsync(id);
            if (reporte == null)
            {
                return NotFound();
            }

            return Ok(reporte);
        }

        [HttpPost]
        [Authorize(Roles ="Aprendiz")]
        public async Task<ActionResult<ReportResDto>> PostReport(ReportReqDto req)
        {
            int AprendizId = GetUserIdFromToken();

            var ambienteActivo = await service.ReporteActivo(req.IdAmbiente);

            if (ambienteActivo)
            {
                return BadRequest("El ambiente ya tiene un reporte activo");
            }

            if (censorship.EsSoez(req.Descripcion) || censorship.EsSoez(req.Titulo))
            {
                return BadRequest("El contenido del reporte no puede contener palabras soeces");
            }

            var reporte = await service.PostReportAsync(req, AprendizId);
            if (reporte == null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(GetReport), new {id = reporte.Id}, reporte);
        }

        [HttpPut("{id}")]
        [Authorize(Roles ="Administrador")]
        public async Task<ActionResult> UpdateEstado(int id)
        {
            var actualizado = await service.UpdateReportAsync(id);
            if (!actualizado)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("/Estadisticas")]
        [Authorize(Roles= "Administrador")]
        public async Task<ActionResult<StatsReportDto>> GetEstadisticas()
        {
            return Ok(service.GetStats());
        } 

        [HttpGet("/ReportsExcel")]
        [Authorize(Roles= "Administrador")]
        public async Task<IActionResult> GetExcelReports()
        {
            var bytes = await service.ExportarExcelMesActualAsync();

            var nombre = $"reportes_{DateTime.UtcNow:yyyy_MM}.xlsx";

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                nombre
            );
        }


        private int GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim) || !int.TryParse(claim, out var id))
            {
                return 0;
            }
            return id;
        }
    }
}
