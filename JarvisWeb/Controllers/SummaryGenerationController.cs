using JarvisWeb.Models;
using JarvisWeb.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JarvisWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryGenerationController(ILogger<SummaryGenerationController> logger, SummaryGenerationService summaryGenerationService) : ControllerBase
    {
        private readonly ILogger<SummaryGenerationController> _logger = logger;
        private readonly SummaryGenerationService _summaryGenerationService = summaryGenerationService;


        [HttpPost]
        public async Task<IActionResult> GenerateSummary(SummaryGenerationRequest request)
        {
            _logger.LogInformation("Generating summary for {0}", request);

            var summary = await _summaryGenerationService.GenerateSummaryAsync(new Services.Models.SummaryGenerationRequest());

            return Ok(summary);
        }
    }
}
