using APIFlashCard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIFlashCard.Data;
using APIFlashCard.Utils;

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

            var safeUser = new User
            {
                ID_User = user.ID_User,
                UserName = user.UserName,
                UserPassword = null,
                Is_active = user.Is_active,
                Is_admin = user.Is_admin
            };

            return Ok(safeUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (request == null || request.UserId <= 0 || request.EncryptedPassword == null)
                return BadRequest();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID_User == request.UserId);
            if (user == null || !user.Is_active)
                return BadRequest();

            byte[] hashedPassword;
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                hashedPassword = sha.ComputeHash(request.EncryptedPassword);
            }

            if (user.UserPassword.SequenceEqual(hashedPassword))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
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

            user.UserPassword = PasswordHash.HashAesPassword(user.UserPassword);

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

            user.UserPassword = PasswordHash.HashAesPassword(EncryptedData);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Hasło zmienione pomyślnie" });
        }

        [HttpGet("active/{username}")]
        public async Task<ActionResult<bool>> IsUserActive(string username)
        {
            var user = await _context.Users.Where(u => u.UserName == username).Select(u => new { u.Is_active }).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "Użytkownik nie istnieje." });
            }

            return Ok(user.Is_active);
        }
    }
}