using EcoSENA.Api.Entities;

namespace EcoSENA.Api.Models.Reports
{
    public class ReportExcelDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Ubicacion { get; set; }
        public string Estado { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public DateTime? FechaRevision { get; set; }
        public DateTime? FechaSolucion { get; set; }
        public string Aprendiz { get; set; }
    }
}
