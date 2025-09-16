using APIFlashCard.Data;
using APIFlashCard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFlashCard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailsController : ControllerBase
    {
        private readonly FlashCardDbContext _context;

        public UserDetailsController(FlashCardDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            var userDetails = await _context.UserDetails
                .Where(u => u.ID_User == userId)
                .Select(u => new
                {
                    u.ID_User,
                    u.FirstName,
                    u.LastName,
                    u.Country,
                    u.Avatar
                })
                .FirstOrDefaultAsync();

            if (userDetails == null)
            {
                return NotFound(new { message = "Nie znaleziono użytkownika." });
            }

            return Ok(userDetails);
        }

        [HttpGet("check-email/{email}")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            bool exists = await _context.UserDetails.AnyAsync(u => u.Email == email);
            return Ok(exists ? "exists" : "not_exists");
        }

        [HttpPost]
        public async Task<IActionResult> PostUserDetails(UserDetails userDetails)
        {
            if (userDetails == null)
            {
                return BadRequest("Dane użytkownika są wymagane.");
            }

            _context.UserDetails.Add(userDetails);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserDetails), new { userId = userDetails.ID_User }, userDetails);
        }

        [HttpPut("{userId:int}/avatar")]
        public async Task<IActionResult> UpdateAvatar(int userId, [FromBody] byte[] avatar)
        {
            var user = await _context.UserDetails.FirstOrDefaultAsync(u => u.ID_User == userId);
            if (user == null)
            {
                return NotFound(new { message = "Nie znaleziono użytkownika." });
            }

            user.Avatar = avatar;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Avatar updated successfully." });
        }

        [HttpPut("{userId}/delete-avatar")]
        public async Task<IActionResult> DeleteAvatar(int userId)
        {
            var user = await _context.UserDetails
                             .Where(u => u.ID_User == userId)
                             .SingleOrDefaultAsync();

            if (user == null) return NotFound();

            user.Avatar = null;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}