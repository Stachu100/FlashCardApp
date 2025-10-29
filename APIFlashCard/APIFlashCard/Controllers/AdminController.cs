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

    [HttpPut("users/{id}/toggle")]
    public async Task<IActionResult> ToggleUserActive(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "Użytkownik nie istnieje." });

        user.Is_active = !user.Is_active;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Status użytkownika {user.UserName} zmieniony na {(user.Is_active ? "aktywny" : "nieaktywny")}.", user.Is_active });
    }

    [HttpGet("userdetails/{userId:int}")]
    public async Task<IActionResult> GetUserDetails(int userId)
    {
        var userDetails = await _context.UserDetails
            .Where(u => u.ID_User == userId)
            .Select(u => new
            {
                u.FirstName,
                u.LastName,
                u.Country,
                u.Email
            })
            .FirstOrDefaultAsync();

        if (userDetails == null)
        {
            return NotFound(new { message = "Nie znaleziono użytkownika." });
        }

        return Ok(userDetails);
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

    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category updatedCategory)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return NotFound(new { message = "Kategoria nie istnieje." });

        category.CategoryName = updatedCategory.CategoryName;
        category.FrontLanguage = updatedCategory.FrontLanguage;
        category.BackLanguage = updatedCategory.BackLanguage;
        category.LanguageLevel = updatedCategory.LanguageLevel;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Kategoria została zaktualizowana." });
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var flashcards = await _context.FlashCards
            .Where(f => f.ID_Category == id)
            .ToListAsync();

        if (flashcards.Any())
        {
            _context.FlashCards.RemoveRange(flashcards);
            await _context.SaveChangesAsync();
        }

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound(new { message = "Nie znaleziono kategorii." });
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Kategoria oraz jej fiszki zostały usunięte." });
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

    [HttpGet("notifications")]
    public async Task<IActionResult> Getnotifications()
    {
        var notifications = await _context.Notifications
            .OrderByDescending(n => n.Is_read == null)
            .Take(200)
            .ToListAsync();
        return Ok(notifications);
    }

    [HttpPut("Readnotifications")]
    public async Task<IActionResult> Readnotifications()
    {
        var notifications = _context.Notifications.ToList();

        foreach (var not in notifications) {
            not.Is_read = true;
        }   

        await _context.SaveChangesAsync();

        return Ok();
    }
}