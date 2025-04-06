using JarvisWeb.Models;
using JarvisWeb.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace JarvisWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(
        UserService userService,
        GlobalStateService globalStateService
        ) : Controller
    {
        private readonly UserService _userService = userService;
        private readonly GlobalStateService _globalStateService = globalStateService;

        [HttpPost]
        [Route("update-is-in-office")]
        public async Task<IActionResult> UpdateUserIsInOffice(UserInOfficeRequestModel model, [FromHeader] string authorization)
        {
            var user = await _userService.GetUserByApiKey(authorization);
            if (user == null)
            {
                return Unauthorized();
            }
            _userService.UpdateUserIsInOffice(user.Id, model.IsInOffice);
            return Ok();
        }

        [HttpPost]
        [Route("reset")]
        public async Task<IActionResult> ResetUserInformation([FromHeader] string authorization)
        {
            var user = await _userService.GetUserByApiKey(authorization);
            if (user == null)
            {
                return Unauthorized();
            }
            user.IsInOffice = false;
            user.LastSeenDailySummaryId = null;
            await _userService.Update(user);
            _globalStateService.StateHasChanged(user.Id);
            return Ok();
        }
    }
}
