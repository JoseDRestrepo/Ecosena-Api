namespace EcoSENA.Api.Models.Reports
{
    public class StatsReportDto
    {
        public int ReportesHechos { get; set; }
        public double ReportesPendientes { get; set; }
        public double ReportesEnProgreso { get; set; }
        public double ReportesResueltos { get; set; }
    }
}
