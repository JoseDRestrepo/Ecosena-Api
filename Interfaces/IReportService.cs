using EcoSENA.Api.Models.Reports;

namespace EcoSENA.Api.Interfaces
{
    public interface IReportService
    {
        public Task<List<ReportListResDto>> GetReportsAsync();
        public Task<List<ReportListResDto>> GetReportsAprendizAsync(int id);
        public Task<ReportResDto> GetReportAsync(int id);
        public Task<ReportResDto> PostReportAsync(ReportReqDto req, int redactorId);
        public Task<bool> UpdateReportAsync(int id);
    }
}
