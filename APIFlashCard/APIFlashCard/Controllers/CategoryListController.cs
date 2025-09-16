using APIFlashCard.Data;
using APIFlashCard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFlashCard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryListController : ControllerBase
    {
        private readonly FlashCardDbContext _context;

        public CategoryListController(FlashCardDbContext context)
        {
            _context = context;
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchCategories([FromBody] CategoryFilter filters)
        {
            var query = _context.Categories.Include(c => c.User).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.CategoryName))
                query = query.Where(c => EF.Functions.Like(c.CategoryName, $"%{filters.CategoryName}%"));

            if (!string.IsNullOrWhiteSpace(filters.UserName))
            {
                var usernameNormalized = filters.UserName.ToLower().Trim();
                query = query.Where(c => c.User.UserName.ToLower() == usernameNormalized);
            }

            if (!string.IsNullOrWhiteSpace(filters.LanguageLevel))
                query = query.Where(c => c.LanguageLevel == filters.LanguageLevel);

            if (!string.IsNullOrWhiteSpace(filters.UserLanguage))
                query = query.Where(c => c.FrontLanguage == filters.UserLanguage || c.BackLanguage == filters.UserLanguage);

            var categories = await query
                .Include(c => c.User)
                .Select(c => new
                {
                    c.ID_Category,
                    c.CategoryName,
                    c.FrontLanguage,
                    c.BackLanguage,
                    c.LanguageLevel,
                    c.User.UserName
                })
                .ToListAsync();

            return Ok(categories);
        }
    }
}