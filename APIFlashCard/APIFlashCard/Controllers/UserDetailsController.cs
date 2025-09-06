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
    }
}