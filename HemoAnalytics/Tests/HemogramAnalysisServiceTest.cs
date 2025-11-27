using Xunit;
using HemoAnalytics.Services;
using HemoAnalytics.Dto;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HemoAnalytics.Tests
{
    public class HemogramAnalysisServiceTest
    {
        private readonly HemogramAnalysisService _service;

        public HemogramAnalysisServiceTest()
        {
            _service = new HemogramAnalysisService();
        }

        [Fact]
        public async Task AnalyzeRiskAndAlertAsync_ShouldGenerateAlert_WhenValuesAreCritical()
        {
            var counts = new BloodCounts
            {
                MonocyteCount = 2.0,
                NeutrophilCount = 3.0,
                LymphocyteCount = 1.0,
                PlateletCount = 100.0
            };

            var result = await _service.AnalyzeRiskAndAlertAsync(counts);

            Assert.True(result.AlertGenerated);
            Assert.Contains("elevado", result.AlertReason);
            Assert.NotEmpty(result.AlertReasons);
        }

        [Fact]
        public async Task AnalyzeRiskAndAlertAsync_ShouldNotGenerateAlert_WhenValuesAreNormal()
        {
            var counts = new BloodCounts
            {
                MonocyteCount = 0.2,
                NeutrophilCount = 1.0,
                LymphocyteCount = 2.0,
                PlateletCount = 500.0
            };

            var result = await _service.AnalyzeRiskAndAlertAsync(counts);

            Assert.False(result.AlertGenerated);
            Assert.Contains("normal", result.AlertReason);
            Assert.Empty(result.AlertReasons);
        }

        [Fact]
        public async Task AnalyzeRiskAndAlertAsync_ShouldReturnCorrectRatios()
        {
            var counts = new BloodCounts
            {
                MonocyteCount = 1.0,
                NeutrophilCount = 2.0,
                LymphocyteCount = 2.0,
                PlateletCount = 200.0
            };

            var result = await _service.AnalyzeRiskAndAlertAsync(counts);

            Assert.Equal(1.0, result.Ratios.MLR, 3);
            Assert.Equal(1.0, result.Ratios.SIRI, 3);
            Assert.Equal(1.0, result.Ratios.LMR, 3);
            Assert.Equal(100.0, result.Ratios.PLR, 3);
            Assert.Equal(1.0, result.Ratios.LPR100, 3);
        }
    }
}