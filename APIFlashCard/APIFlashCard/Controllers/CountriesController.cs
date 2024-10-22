﻿using APIFlashCard.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using APIFlashCard.Models;

namespace APIFlashCard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly FlashCardDbContext _context;

        public CountriesController(FlashCardDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Countries>>> GetAllCountries()
        {
            return await _context.Countries.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Countries>> GetCountryById(int id)
        {
            var country = await _context.Countries.FindAsync(id);

            if (country == null)
            {
                return NotFound();
            }

            return country;
        }

        [HttpGet("{countryName}")]
        public async Task<ActionResult<Countries>> GetCountryByName(string countryName)
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(c => c.Country.ToLower() == countryName.ToLower());

            if (country == null)
            {
                return NotFound();
            }

            return country;
        }
    }
}
