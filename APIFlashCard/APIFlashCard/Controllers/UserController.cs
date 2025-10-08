using APIFlashCard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIFlashCard.Data;

namespace APIFlashCard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly FlashCardDbContext _context;

        public UserController(FlashCardDbContext context)
        {
            _context = context;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<User>> GetUserByUsername(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("check-username/{username}")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            bool exists = await _context.Users.AnyAsync(u => u.UserName == username);
            return Ok(exists ? "exists" : "not_exists");
        }

        [HttpPost]
        public async Task<IActionResult> PostUser(User user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Dane użytkownika są wymagane." });
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserByUsername), new { username = user.UserName }, user);
        }

        [HttpPut("{userId:int}/ChangePassword")]
        public async Task<IActionResult> ChangePassowrdUser(int userId, [FromBody] byte[] EncryptedData)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID_User == userId);
            if (user == null)
            {
                return NotFound(new { message = "Nie znaleziono użytkownika." });
            }

            user.UserPassword = EncryptedData;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Hasło zmienione pomyślnie" });
        }
    }
}