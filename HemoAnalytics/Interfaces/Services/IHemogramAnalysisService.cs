using HemoAnalytics.Dto;

namespace HemoAnalytics.Interfaces.Services
{
    public interface IHemogramAnalysisService
    {
        Task<AnalysisResult> AnalyzeRiskAndAlertAsync(BloodCounts counts);
    }
}
