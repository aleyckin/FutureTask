using Contracts.Dtos.MetricsDtos;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
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
        private readonly IMetricsService _metricsService;
        private readonly IMetricsRepository _metricsRepository;

        public MetricsController(IMetricsRepository metricsRepository, IMetricsService metricsService)
        {
            _metricsRepository = metricsRepository;
            _metricsService = metricsService;
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

        [HttpGet("ReportForProject/{projectId:guid}")]
        public async Task<IActionResult> GetProjectReport([FromRoute] Guid projectId)
        {
            byte[] pdfBytes = await _metricsService.GetReportForProject(projectId);
            return File(pdfBytes, "application/pdf", $"Report_{projectId}.pdf");
        }

        [HttpGet("ReportForUser")]
        public async Task<IActionResult> GetUserReport()
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            Guid userId = Guid.Parse(userIdClaim.Value);
            byte[] pdfBytes = await _metricsService.GetReportForUser(userId);
            return File(pdfBytes, "application/pdf", $"Report_{userId}.pdf");
        }
    }
}
