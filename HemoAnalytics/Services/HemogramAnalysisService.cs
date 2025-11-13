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
            // Cálculo dos índices
            var ratios = new CoronaryRiskRatios
            {
                SIRI = (counts.MonocyteCount * counts.NeutrophilCount) / counts.LymphocyteCount,
                MLR = counts.MonocyteCount / counts.LymphocyteCount,
                LPR100 = (counts.LymphocyteCount / counts.PlateletCount) * 100,
                LMR = counts.LymphocyteCount / counts.MonocyteCount,
                PLR = counts.PlateletCount / counts.LymphocyteCount
            };

            // Lógica de alerta e status detalhado
            bool alert = false;
            var statusMsg = new System.Text.StringBuilder();
            var culture = CultureInfo.InvariantCulture;
            var status = new Dictionary<string, string>();
            var alertReasons = new List<string>();

            // SIRI
            string siriStatus = ratios.SIRI >= 1.462 ? "elevado" : "normal";
            status["siri"] = siriStatus;
            if (ratios.SIRI >= 1.462) {
                alert = true;
                alertReasons.Add("SIRI elevado (>= 1,462)");
            }
            statusMsg.AppendLine($"SIRI: {ratios.SIRI.ToString("F3", culture)} - {siriStatus} (corte: 1,462)");

            // MLR
            string mlrStatus = ratios.MLR > 0.4 ? "elevado" : "normal";
            status["mlr"] = mlrStatus;
            if (ratios.MLR > 0.4) {
                alert = true;
                alertReasons.Add("MLR elevado (> 0,4)");
            }
            statusMsg.AppendLine($"MLR: {ratios.MLR.ToString("F3", culture)} - {mlrStatus} (corte: 0,4)");

            // LPR100
            string lpr100Status = ratios.LPR100 > 4.0 ? "elevado" : "normal";
            status["lpr100"] = lpr100Status;
            if (ratios.LPR100 > 4.0) {
                alert = true;
                alertReasons.Add("LPR*100 elevado (> 4,0)");
            }
            statusMsg.AppendLine($"LPR*100: {ratios.LPR100.ToString("F3", culture)} - {lpr100Status} (corte: 4,0)");

            // LMR
            string lmrStatus = ratios.LMR < 3.75 ? "baixo" : "normal";
            status["lmr"] = lmrStatus;
            if (ratios.LMR < 3.75) {
                alert = true;
                alertReasons.Add("LMR baixo (< 3,75)");
            }
            statusMsg.AppendLine($"LMR: {ratios.LMR.ToString("F3", culture)} - {lmrStatus} (corte: 3,75)");

            // PLR
            string plrStatus = ratios.PLR < 185.714 ? "baixo" : "normal";
            status["plr"] = plrStatus;
            if (ratios.PLR < 185.714) {
                alert = true;
                alertReasons.Add("PLR baixo (< 185,714)");
            }
            statusMsg.AppendLine($"PLR: {ratios.PLR.ToString("F3", culture)} - {plrStatus} (corte: 185,714)");

            return new AnalysisResult
            {
                Ratios = ratios,
                AlertGenerated = alert,
                AlertReason = statusMsg.ToString().Trim(),
                Status = status,
                AlertReasons = alertReasons
            };
        }
    }
}
