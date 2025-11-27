using HemoAnalytics.Dto;
using HemoAnalytics.Interfaces.Services;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Generic;

namespace HemoAnalytics.Services
{
    public class HemogramAnalysisService : IHemogramAnalysisService
    {
        public async Task<AnalysisResult> AnalyzeRiskAndAlertAsync(BloodCounts counts)
        {
            // Cálculo dos índices usando métodos privados
            var ratios = new CoronaryRiskRatios
            {
                SIRI = CalculateSIRI(counts),
                MLR = CalculateMLR(counts),
                LPR100 = CalculateLPR100(counts),
                LMR = CalculateLMR(counts),
                PLR = CalculatePLR(counts)
            };

            // Lógica de alerta e status detalhado movida para métodos privados
            var (alert, status, alertReasons) = AnalyzeIndices(ratios);
            var statusMsg = BuildStatusMessage(ratios, status);

            return new AnalysisResult
            {
                Ratios = ratios,
                AlertGenerated = alert,
                AlertReason = statusMsg.Trim(),
                Status = status,
                AlertReasons = alertReasons
            };
        }

        private (bool alert, Dictionary<string, string> status, List<string> alertReasons) AnalyzeIndices(CoronaryRiskRatios ratios)
        {
            bool alert = false;
            var status = new Dictionary<string, string>();
            var alertReasons = new List<string>();

            // SIRI
            string siriStatus = ratios.SIRI >= 1.462 ? "elevado" : "normal";
            status["siri"] = siriStatus;
            if (ratios.SIRI >= 1.462) {
                alert = true;
                alertReasons.Add("SIRI elevado (>= 1,462)");
            }

            // MLR
            string mlrStatus = ratios.MLR > 0.4 ? "elevado" : "normal";
            status["mlr"] = mlrStatus;
            if (ratios.MLR > 0.4) {
                alert = true;
                alertReasons.Add("MLR elevado (> 0,4)");
            }

            // LPR100
            string lpr100Status = ratios.LPR100 > 4.0 ? "elevado" : "normal";
            status["lpr100"] = lpr100Status;
            if (ratios.LPR100 > 4.0) {
                alert = true;
                alertReasons.Add("LPR*100 elevado (> 4,0)");
            }

            // LMR
            string lmrStatus = ratios.LMR < 3.75 ? "baixo" : "normal";
            status["lmr"] = lmrStatus;
            if (ratios.LMR < 3.75) {
                alert = true;
                alertReasons.Add("LMR baixo (< 3,75)");
            }

            // PLR
            string plrStatus = ratios.PLR < 185.714 ? "baixo" : "normal";
            status["plr"] = plrStatus;
            if (ratios.PLR < 185.714) {
                alert = true;
                alertReasons.Add("PLR baixo (< 185,714)");
            }

            return (alert, status, alertReasons);
        }

        // Método privado para construir a mensagem de status
        private string BuildStatusMessage(CoronaryRiskRatios ratios, Dictionary<string, string> status)
        {
            var culture = CultureInfo.InvariantCulture;
            var statusMsg = new System.Text.StringBuilder();
            statusMsg.AppendLine($"SIRI: {ratios.SIRI.ToString("F3", culture)} - {status["siri"]} (corte: 1,462)");
            statusMsg.AppendLine($"MLR: {ratios.MLR.ToString("F3", culture)} - {status["mlr"]} (corte: 0,4)");
            statusMsg.AppendLine($"LPR*100: {ratios.LPR100.ToString("F3", culture)} - {status["lpr100"]} (corte: 4,0)");
            statusMsg.AppendLine($"LMR: {ratios.LMR.ToString("F3", culture)} - {status["lmr"]} (corte: 3,75)");
            statusMsg.AppendLine($"PLR: {ratios.PLR.ToString("F3", culture)} - {status["plr"]} (corte: 185,714)");
            return statusMsg.ToString();
        }

        // Métodos privados para cálculo dos índices
        private double CalculateSIRI(BloodCounts counts)
        {
            return (counts.MonocyteCount * counts.NeutrophilCount) / counts.LymphocyteCount;
        }

        private double CalculateMLR(BloodCounts counts)
        {
            return counts.MonocyteCount / counts.LymphocyteCount;
        }

        private double CalculateLPR100(BloodCounts counts)
        {
            return (counts.LymphocyteCount / counts.PlateletCount) * 100;
        }

        private double CalculateLMR(BloodCounts counts)
        {
            return counts.LymphocyteCount / counts.MonocyteCount;
        }

        private double CalculatePLR(BloodCounts counts)
        {
            return counts.PlateletCount / counts.LymphocyteCount;
        }
    }
}
