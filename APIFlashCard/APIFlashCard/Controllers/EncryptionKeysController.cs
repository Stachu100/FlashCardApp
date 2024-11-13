﻿using APIFlashCard.Data;
using APIFlashCard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIFlashCard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptionKeysController : ControllerBase
    {
        private readonly FlashCardDbContext _context;

        public EncryptionKeysController(FlashCardDbContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<EncryptionKeys>> GetEncryptionKeys(int userId)
        {
            var encryptionKeys = await _context.EncryptionKeys
                .Where(k => k.ID_User == userId)
                .FirstOrDefaultAsync();

            if (encryptionKeys == null)
            {
                return NotFound("Klucze szyfrowania dla użytkownika nie zostały znalezione.");
            }

            return encryptionKeys;
        }

        [HttpPost]
        public async Task<ActionResult<EncryptionKeys>> PostEncryptionKeys(EncryptionKeys encryptionKeys)
        {
            if (encryptionKeys == null)
            {
                return BadRequest("Dane klucza szyfrowania są wymagane.");
            }

            _context.EncryptionKeys.Add(encryptionKeys);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEncryptionKeys), new { userId = encryptionKeys.ID_User }, encryptionKeys);
        }
    }
}