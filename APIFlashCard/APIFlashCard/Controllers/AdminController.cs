using APIFlashCard.Data;
using APIFlashCard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly FlashCardDbContext _context;

    public AdminController(FlashCardDbContext context)
    {
        _context = context;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetLogs()
    {
        var logs = await _context.Logs
            .OrderByDescending(l => l.TimeStamp)
            .Take(200)
            .ToListAsync();
        return Ok(logs);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories.ToListAsync();
        return Ok(categories);
    }

    [HttpPost("categories")]
    public async Task<IActionResult> AddCategory([FromBody] Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return Ok(category);
    }

    [HttpGet("flashcards/{categoryId:int}")]
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

    [HttpPost("flashcards")]
    public async Task<IActionResult> AddFlashCard([FromBody] FlashCard flashCard)
    {
        if (flashCard == null || string.IsNullOrWhiteSpace(flashCard.FrontFlashCard))
        {
            return BadRequest(new { message = "Nieprawidłowe dane fiszki." });
        }

        _context.FlashCards.Add(flashCard);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(AddFlashCard), new { id = flashCard.ID_flashcard }, flashCard);
    }
}