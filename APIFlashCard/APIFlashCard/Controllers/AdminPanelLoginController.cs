using APIFlashCard.Data;
using APIFlashCard.Models;
using APIFlashCard.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFlashCard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminPanelLoginController : ControllerBase
    {
        private readonly FlashCardDbContext _context;

        public AdminPanelLoginController(FlashCardDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminPanelLogin request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Please provide username and password." });

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == request.Username);

            if (user == null)
                return Unauthorized(new { message = "Invalid username or password." });

            if (!user.Is_admin)
                return Unauthorized(new { message = "Your account does not have administrator privileges." });

            var encryptionKeys = await _context.EncryptionKeys
                .FirstOrDefaultAsync(k => k.ID_User == user.ID_User);

            if (encryptionKeys == null)
                return Unauthorized(new { message = "Invalid username or password." });

            try
            {
                string decryptedPassword = AesHelper.DecryptStringFromBytes_Aes(
                    user.UserPassword,
                    encryptionKeys.EncryptionKey,
                    encryptionKeys.IV
                );

                if (!user.Is_active)
                    return Unauthorized(new { message = "Invalid username or password." });

                if (decryptedPassword != request.Password)
                    return Unauthorized(new { message = "Invalid username or password." });

                return Ok(new { message = "Logged in successfully", userId = user.ID_User });
            }
            catch
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }
        }
    }
}