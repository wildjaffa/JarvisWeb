using JarvisWeb.Models;
using JarvisWeb.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JarvisWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailySummaryController(
        ILogger<DailySummaryController> logger, 
        DailySummaryService dailySummaryService,
        IHttpContextAccessor httpContextAccessor,
        UserService userService) : ControllerBase
    {
        private readonly ILogger<DailySummaryController> _logger = logger;
        private readonly DailySummaryService _dailySummaryService = dailySummaryService;
        private readonly IHttpContextAccessor _contextAccessor = httpContextAccessor;
        private readonly UserService _userService = userService;


        [HttpPost]
        [Route("generate-summary")]
        public async Task<IActionResult> GenerateSummary([FromHeader] string authorization)
        {
            if (authorization == null)
            {
                return Unauthorized();
            }
            var user = await _userService.GetUserByApiKey(authorization);
            if (user == null)
            {
                return Unauthorized();
            }
            _logger.LogInformation("Generating summary for {userId}", user.Id);

            var generationRequest = new Services.Models.SummaryGenerationRequest
            {
                UserId = user.Id,
                GenerateVideo = false
            };
            var summary = await _dailySummaryService.GenerateSummaryAsync(generationRequest);

            return Ok(summary);
        }

        [HttpPost]
        [Route("generate-video")]
        public async Task<IActionResult> GenerateVideo([FromHeader] string authorization)
        {
            if (authorization == null)
            {
                return Unauthorized();
            }
            var user = await _userService.GetUserByApiKey(authorization);
            if (user == null)
            {
                return Unauthorized();
            }
            _logger.LogInformation("Generating summary video for {userId}", user.Id);
            _dailySummaryService.GenerateSummaryVideo(user.Id);
            return Ok("Generation Started");
        }

        [HttpGet]
        [Route("get-video")]
        public async Task<IActionResult> GetDailySummaryVideo([FromQuery] string summaryId)
        {
            var httpContext = _contextAccessor.HttpContext;
            var token = httpContext.Request.Cookies["idToken"];
            var user = await _userService.GetUserFromJwtToken(token);
            if (user == null)
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(summaryId, out var parsedSummaryID))
            {
                return BadRequest("summaryId is not in correct format");
            }

            var dailySummary = await _dailySummaryService.GetByIdAsync(user.Id, parsedSummaryID);
            if (dailySummary == null)
            {
                return NotFound("A summary coud not be found with that id");
            }

            if (!System.IO.File.Exists(dailySummary.SummaryVideoPath)) 
            {
                return NotFound("A video could not be found for the summary");
            }

            return PhysicalFile(dailySummary.SummaryVideoPath, "video/mpeg");
        }
    }
}
