using APIFlashCard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIFlashCard.Data;

namespace APIFlashCard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCountriesController : ControllerBase
    {
        private readonly FlashCardDbContext _context;

        public UserCountriesController(FlashCardDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserCountries>>> GetUserCountries()
        {
            var userCountries = await _context.UserCountries.ToListAsync();
            return Ok(userCountries);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserCountries>> GetUserCountry(int id)
        {
            var userCountry = await _context.UserCountries.FindAsync(id);

            if (userCountry == null)
            {
                return NotFound();
            }

            return Ok(userCountry);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<IEnumerable<UserCountries>>> GetUserCountriesByUserId(int userId)
        {
            var userCountries = await _context.UserCountries
                                              .Where(uc => uc.ID_User == userId)
                                              .ToListAsync();

            if (userCountries == null || userCountries.Count == 0)
            {
                return NotFound();
            }

            return Ok(userCountries);
        }

        [HttpPost]
        public async Task<IActionResult> PostUserCountry(UserCountries userCountry)
        {
            if (userCountry == null)
            {
                return BadRequest("Nieprawidłowe dane");
            }

            _context.UserCountries.Add(userCountry);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserCountry), new { id = userCountry.ID_UserCountries }, userCountry);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUserCountry(int userId, int countryId)
        {
            var userCountry = await _context.UserCountries
                                            .FirstOrDefaultAsync(uc => uc.ID_User == userId && uc.ID_Country == countryId);

            if (userCountry == null)
            {
                return NotFound();
            }

            _context.UserCountries.Remove(userCountry);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
