namespace EcoSENA.Api.Models.Reports
{
    public class StatsReportDto
    {
        public int ReportesHechosMes { get; set; }
        public int ReportesPendientes { get; set; }
        public int ReportesEnProgreso { get; set; }
        public int ReportesResueltosMes { get; set; }
    }
}
