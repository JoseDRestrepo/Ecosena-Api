namespace EcoSENA.Api.Models.Reports
{
    public class ReportReqDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int IdAmbiente { get; set; } 
        public required IFormFile Foto { get; set; }
    }
}
