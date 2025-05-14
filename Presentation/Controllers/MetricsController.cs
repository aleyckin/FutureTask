using Contracts.Dtos.MetricsDtos;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/metrics")]
    [Authorize]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        private readonly IMetricsRepository _metricsRepository;
        public MetricsController(IMetricsRepository metricsRepository)
        {
            _metricsRepository = metricsRepository;
        }

        [HttpPost("task")]
        public async Task<IActionResult> LogTaskMetrics([FromBody] TaskMetricsDto dto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = new Guid(userIdClaim);

            var entity = new TaskMetrics
            {
                UserId = userId,
                OpenToRec = dto.OpenToRec,
                RecResponseTime = dto.RecResponseTime,
                RecToSave = dto.RecToSave,
                TotalTime = dto.TotalTime,
                UsedRecommendations = dto.UsedRecommendations,
                Timestamp = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
            };


            await _metricsRepository.AddAsync(entity);
            return Ok();
        }
    }
}
