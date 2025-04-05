using JarvisWeb.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JarvisWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EndOfDayNoteController(
        ILogger<EndOfDayNoteController> logger,
        EndOfDayNoteService endOfDayNoteService,
        IHttpContextAccessor httpContextAccessor,
        UserService userService
    ) : ControllerBase
    {
        private readonly ILogger<EndOfDayNoteController> _logger = logger;
        private readonly EndOfDayNoteService _endOfDayNoteService = endOfDayNoteService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserService _userService = userService;

        [HttpPost]
        [Route("from-audio")]
        public async Task<IActionResult> CreateEndOfDayNoteFromAudioFile([FromForm] IFormFile audio)
        {
            var userToken = _httpContextAccessor.HttpContext.Request.Cookies["idToken"];
            if (userToken == null)
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserFromJwtToken(userToken);
            if (user == null)
            {
                return Unauthorized();
            }

            var note = await _endOfDayNoteService.CreateEndOfDayNoteFromAudioFile(user.Id, audio);
            if (note == null)
            {
                return Problem("Unable to generate note");
            }
            return Ok(note);
        }
    }
}
