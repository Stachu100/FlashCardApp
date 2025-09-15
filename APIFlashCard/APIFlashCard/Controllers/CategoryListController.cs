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
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.CategoryName))
                query = query.Where(c => EF.Functions.Like(c.CategoryName, $"%{filters.CategoryName}%"));

            if (!string.IsNullOrWhiteSpace(filters.UserName))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == filters.UserName);
                if (user != null)
                    query = query.Where(c => c.UserID == user.ID_User);
                else
                    return Ok(new List<Category>());
            }

            if (!string.IsNullOrWhiteSpace(filters.LanguageLevel))
                query = query.Where(c => c.LanguageLevel == filters.LanguageLevel);

            if (!string.IsNullOrWhiteSpace(filters.UserLanguage))
                query = query.Where(c => c.FrontLanguage == filters.UserLanguage || c.BackLanguage == filters.UserLanguage);

            var categories = await query.ToListAsync();
            return Ok(categories);
        }
    }
}