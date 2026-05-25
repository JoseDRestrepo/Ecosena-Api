namespace EcoSENA.Api.Models
{
    public class ReportListResDto
    {
        public int Id { get; set; }
        public string Ubicacion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateOnly FechaEmision { get; set; }
    }
}
