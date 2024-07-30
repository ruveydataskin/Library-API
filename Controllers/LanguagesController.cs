using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI6.Data;
using LibraryAPI6.Models;
using Microsoft.AspNetCore.Authorization;

namespace LibraryAPI6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguagesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public LanguagesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Languages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Language>>> GetLanguages()
        {
            if (_context.Languages == null)
            {
                return NotFound(); // Return 404 if Languages DbSet is null
            }
            return await _context.Languages.ToListAsync(); // Retrieve all Languages
        }

        // GET: api/Languages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Language>> GetLanguage(string id)
        {
            if (_context.Languages == null)
            {
                return NotFound(); // Return 404 if Languages DbSet is null
            }

            var language = await _context.Languages.FindAsync(id);

            if (language == null)
            {
                return NotFound(); // Return 404 if the specific Language is not found
            }

            return language; // Return the found Language
        }

        // PUT: api/Languages/5
        [Authorize(Roles = "Worker, Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLanguage(string id, Language language)
        {
            if (id != language.Code)
            {
                return BadRequest(); // Return 400 if the ID in the URL does not match the ID in the body
            }

            _context.Entry(language).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LanguageExists(id))
                {
                    return NotFound(); // Return 404 if the specific Language is not found
                }
                else
                {
                    throw; // Rethrow if a different concurrency exception occurred
                }
            }

            return NoContent(); // Return 204 No Content if the update is successful
        }

        // POST: api/Languages
        [Authorize(Roles = "Worker, Admin")]
        [HttpPost]
        public async Task<ActionResult<Language>> PostLanguage(Language language)
        {
            if (_context.Languages == null)
            {
                return Problem("Entity set 'ApplicationContext.Languages' is null."); // Return 500 if Languages DbSet is null
            }

            _context.Languages.Add(language);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LanguageExists(language.Code))
                {
                    return Conflict(); // Return 409 Conflict if the Language already exists
                }
                else
                {
                    throw; // Rethrow if a different DbUpdateException occurred
                }
            }

            return CreatedAtAction("GetLanguage", new { id = language.Code }, language); // Return 201 Created with the location of the new resource
        }

        // DELETE: api/Languages/5
        [Authorize(Roles = "Worker, Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLanguage(string id)
        {
            if (_context.Languages == null)
            {
                return NotFound(); // Return 404 if Languages DbSet is null
            }

            var language = await _context.Languages.FindAsync(id);

            if (language == null)
            {
                return NotFound(); // Return 404 if the specific Language is not found
            }

            _context.Languages.Remove(language);
            await _context.SaveChangesAsync();

            return NoContent(); // Return 204 No Content if the deletion is successful
        }

        private bool LanguageExists(string id)
        {
            return (_context.Languages?.Any(e => e.Code == id)).GetValueOrDefault(); // Check if the Language exists
        }
    }
}
