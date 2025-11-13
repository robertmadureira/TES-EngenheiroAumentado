using HemoAnalytics.Dto;
using HemoAnalytics.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HemoAnalytics.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HemoAnalyticsController : ControllerBase
    {
        private readonly IHemogramAnalysisService _analysisService;
        public HemoAnalyticsController
        (
            IHemogramAnalysisService analysisService
        )
        {
            _analysisService = analysisService ?? throw new ArgumentNullException(nameof(_analysisService));
        }

        [HttpPost("analyze")] 
        public async Task<IActionResult> AnalyzeHemogram([FromBody] BloodCounts bloodCounts)
        {
            // Validação básica (o ASP.NET Core faz muito disso se você usar DataAnnotations)
            if (bloodCounts.LymphocyteCount <= 0 || bloodCounts.MonocyteCount <= 0 || bloodCounts.PlateletCount <= 0)
            {
                return BadRequest("Contagens de células inválidas. Denominadores não podem ser zero.");
            }

            // 1. Chame o serviço para fazer a análise completa
            // O serviço agora é assíncrono (Task)
            var analysisResult = await _analysisService.AnalyzeRiskAndAlertAsync(bloodCounts);

            // 2. Retorne uma resposta com base no que o serviço fez
            return Accepted(new
            {
                Message = "Hemograma recebido e processado.",
                Ratios = analysisResult.Ratios,
                AlertGenerated = analysisResult.AlertGenerated,
                AlertReasons = analysisResult.AlertReasons,
                Status = analysisResult.Status
            });
        }
    }
}
