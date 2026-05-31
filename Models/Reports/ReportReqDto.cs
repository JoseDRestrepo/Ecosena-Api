namespace EcoSENA.Api.Models.Reports
{
    public class ReportReqDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public required IFormFile Foto { get; set; }
    }
}
