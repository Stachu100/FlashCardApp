using APIFlashCard.Data;
using APIFlashCard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFlashCard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly FlashCardDbContext _context;

        public LogsController(FlashCardDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Log>>> GetLogs()
        {
            return await _context.Logs
                .OrderByDescending(l => l.TimeStamp)
                .Take(200)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Log>> CreateLog(Log log)
        {
            log.TimeStamp = DateTime.Now;
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLogs), new { id = log.Id }, log);
        }
    }
}