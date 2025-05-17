using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos.MetricsDtos
{
    public record TaskMetricsDto(double OpenToRec, double RecResponseTime, double RecToSave, double TotalTime, bool UsedRecommendations, DateTime? TimeStamp) { }
}
