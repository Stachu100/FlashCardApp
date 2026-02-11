using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIFlashCard.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/auth")]
    public class AuthInfoController : ControllerBase
    {
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                username = User.Identity!.Name
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookie");
            return Ok();
        }
    }
}