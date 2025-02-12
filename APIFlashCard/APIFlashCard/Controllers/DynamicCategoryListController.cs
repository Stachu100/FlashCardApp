using APIFlashCard.Data;
using APIFlashCard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace APIFlashCard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicCategoryListController : ControllerBase
    {
        private readonly FlashCardDbContext _context;

        public DynamicCategoryListController(FlashCardDbContext context)
        {
            _context = context;
        }

        // Budowanie listy
        [HttpPost("search")]
        public async Task<IActionResult> SearchCategories([FromBody] dynamic filters)
        {
            var query = _context.Categories.AsQueryable();

            try
            {
                JsonElement filtersElement = (JsonElement)filters;

                if (filtersElement.TryGetProperty("CategoryName", out var categoryNameProperty) &&
                    categoryNameProperty.ValueKind != JsonValueKind.Null)
                {
                    string? categoryName = categoryNameProperty.GetString();
                    query = query.Where(c => EF.Functions.Like(c.CategoryName, $"%{categoryName}%"));
                }

                if (filtersElement.TryGetProperty("UserName", out var userNameProperty) &&
                    userNameProperty.ValueKind != JsonValueKind.Null)
                {
                    string? userName = userNameProperty.GetString();
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                    if (user != null)
                    {
                        query = query.Where(c => c.UserID == user.ID_User);                                  // EF.Functions.Like oraz użycie == działa tak samo
                    }                                                                                        // i ustawia dane z filru jako parametr w zapytaniu SQL
                    else
                    {
                        return Ok(new List<Category>());
                    }
                }

                if (filtersElement.TryGetProperty("LanguageLevel", out var languageLevelProperty) &&
                    languageLevelProperty.ValueKind != JsonValueKind.Null)
                {
                    string? languageLevel = languageLevelProperty.GetString();
                    query = query.Where(c => c.LanguageLevel == languageLevel);
                }

                if (filtersElement.TryGetProperty("UserLanguage", out var userLanguageProperty) &&
                    userLanguageProperty.ValueKind != JsonValueKind.Null)
                {
                    string? userLanguage = userLanguageProperty.GetString();
                    query = query.Where(c =>
                        c.FrontLanguage == userLanguage ||
                        c.BackLanguage == userLanguage
                    );
                }

                var categories = await query.ToListAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing request: {ex.Message}");
            }
        }
    }
}