namespace EcoSENA.Api.Models
{
    public class ReportResDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string EmisorReporte { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
