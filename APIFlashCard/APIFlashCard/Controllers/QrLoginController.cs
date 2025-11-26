using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIFlashCard.Data;
using APIFlashCard.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity.Data;

namespace APIFlashCard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QrLoginController : ControllerBase
    {
        private readonly FlashCardDbContext _context;

        public QrLoginController(FlashCardDbContext context)
        {
            _context = context;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateQrToken()
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            //var token = "TEST-TOKEN-1234";

            var now = DateTime.UtcNow;

            var entry = new QrLoginToken
            {
                Token = token,
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(2),
                IsUsed = false
            };

            _context.QrLoginTokens.Add(entry);
            await _context.SaveChangesAsync();

            return Ok(new {token});
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyQrLogin([FromBody] TokenRequest request)
        {
            var tokenEntry = await _context.QrLoginTokens
                .FirstOrDefaultAsync(t => t.Token == request.Token);

            if (tokenEntry == null)
                return BadRequest(new { message = "Niepoprawny token" });

            if (tokenEntry.IsUsed)
                return BadRequest(new { message = "Token został już użyty" });

            if (tokenEntry.ExpiresAt < DateTime.UtcNow)
                return BadRequest(new { message = "Token wygasł" });

            tokenEntry.UserID = request.UserId;
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return BadRequest(new { message = "Niepoprawny użytkownik" });

            return Ok(new
            {
                username = user.UserName,
                message = "Zalogowano pomyślnie"
            });
        }

        [HttpPost("check")]
        public async Task<IActionResult> CheckQrToken([FromBody] TokenCheck request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Token))
                return Ok(new { success = false });

            var tokenEntry = await _context.QrLoginTokens
                .FirstOrDefaultAsync(t => t.Token == request.Token && t.UserID != null && !t.IsUsed);

            if (tokenEntry == null)
                return Ok(new { success = false });

            tokenEntry.IsUsed = true;
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(tokenEntry.UserID.Value);
            var username = user?.UserName ?? string.Empty;

            return Ok(new { success = true, username });
        }
    }
}