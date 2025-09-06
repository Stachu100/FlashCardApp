using APIFlashCard.Data;
using APIFlashCard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFlashCard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashCardController : ControllerBase
    {
        private readonly FlashCardDbContext _context;

        public FlashCardController(FlashCardDbContext context)
        {
            _context = context;
        }

        [HttpPost("batch")]
        public async Task<IActionResult> AddFlashCards([FromBody] List<FlashCard> flashCards)
        {
            if (flashCards == null || !flashCards.Any())
            {
                return BadRequest(new { message = "Lista fiszek jest pusta lub nieprawidłowa." });
            }

            _context.FlashCards.AddRange(flashCards);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Fiszki zostały dodane pomyślnie.", count = flashCards.Count });
        }

        [HttpGet("{categoryId:int}")]
        public async Task<ActionResult<IEnumerable<object>>> GetFlashCardsByCategory(int categoryId)
        {
            var flashcards = await _context.FlashCards
                .Where(f => f.ID_Category == categoryId)
                .Select(f => new { f.FrontFlashCard, f.BackFlashCard })
                .ToListAsync();

            if (!flashcards.Any())
            {
                return NotFound(new { message = "Nie znaleziono fiszek dla tej kategorii." });
            }

            return Ok(flashcards);
        }

        [HttpGet("display/{categoryId}/{page}")]
        public async Task<ActionResult<IEnumerable<FlashCard>>> GetFlashCardsForDisplay(int categoryId, int page = 1)
        {
            const int pageSize = 10;

            var flashcards = await _context.FlashCards
                .Where(f => f.ID_Category == categoryId)
                .OrderBy(f => f.ID_flashcard)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!flashcards.Any())
            {
                return NotFound(new { message = "Brak fiszek do wyświetlenia." });
            }

            return Ok(flashcards);
        }
    }
}