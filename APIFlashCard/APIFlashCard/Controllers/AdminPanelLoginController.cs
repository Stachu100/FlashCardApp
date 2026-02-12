using System.Security.Cryptography;
using APIFlashCard.Data;
using APIFlashCard.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
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
                byte[] encryptedInput = AesHelper.EncryptStringToBytes_Aes(
                    request.Password,
                    encryptionKeys.EncryptionKey,
                    encryptionKeys.IV
                );

                using (var sha = SHA256.Create())
                {
                    byte[] inputHash = sha.ComputeHash(encryptedInput);

                    if (!inputHash.SequenceEqual(user.UserPassword))
                        return Unauthorized(new { message = "Invalid username or password." });
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("UserId", user.ID_User.ToString()),
                    new Claim("IsAdmin", "true")
                };

                var identity = new ClaimsIdentity(claims, "MyCookie");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookie", principal);

                return Ok(new { message = "Logged in successfully", userId = user.ID_User });
            }
            catch
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }
        }
    }
}