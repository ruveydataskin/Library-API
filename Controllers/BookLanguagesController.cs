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
    public class BookLanguagesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BookLanguagesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/BookLanguages
        // Retrieve all BookLanguage records
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookLanguage>>> GetBookLanguages()
        {
            if (_context.BookLanguages == null)
            {
                return NotFound(); // Return 404 if the BookLanguages set is null
            }
            return await _context.BookLanguages.ToListAsync(); // Retrieve and return the list of BookLanguages
        }

        // GET: api/BookLanguages/5
        // Retrieve a specific BookLanguage by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<BookLanguage>> GetBookLanguage(int id)
        {
            if (_context.BookLanguages == null)
            {
                return NotFound(); // Return 404 if the BookLanguages set is null
            }
            var bookLanguage = await _context.BookLanguages.FindAsync(id);

            if (bookLanguage == null)
            {
                return NotFound(); // Return 404 if the BookLanguage is not found
            }

            return bookLanguage; // Return the found BookLanguage
        }

        // PUT: api/BookLanguages/5
        // Update a specific BookLanguage by ID
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker , Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookLanguage(int id, BookLanguage bookLanguage)
        {
            if (id != bookLanguage.BooksId)
            {
                return BadRequest(); // Return 400 if the ID in the URL does not match the ID in the body
            }

            _context.Entry(bookLanguage).State = EntityState.Modified; // Mark the entity as modified

            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookLanguageExists(id))
                {
                    return NotFound(); // Return 404 if the BookLanguage does not exist
                }
                else
                {
                    throw; // Rethrow the exception if it's not a concurrency issue
                }
            }

            return NoContent(); // Return 204 No Content if the update is successful
        }

        // POST: api/BookLanguages
        // Create a new BookLanguage record
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker , Admin")]
        [HttpPost]
        public async Task<ActionResult<BookLanguage>> PostBookLanguage(BookLanguage bookLanguage)
        {
            if (_context.BookLanguages == null)
            {
                return Problem("Entity set 'ApplicationContext.BookLanguages' is null."); // Return 500 if the BookLanguages set is null
            }
            _context.BookLanguages.Add(bookLanguage); // Add the new BookLanguage to the database
            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database
            }
            catch (DbUpdateException)
            {
                if (BookLanguageExists(bookLanguage.BooksId))
                {
                    return Conflict(); // Return 409 Conflict if the BookLanguage already exists
                }
                else
                {
                    throw; // Rethrow the exception if it's not a conflict issue
                }
            }

            return CreatedAtAction("GetBookLanguage", new { id = bookLanguage.BooksId }, bookLanguage); // Return 201 Created with the location of the new resource
        }

        // DELETE: api/BookLanguages/5
        // Delete a specific BookLanguage by ID
        [Authorize(Roles = "Worker , Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookLanguage(int id)
        {
            if (_context.BookLanguages == null)
            {
                return NotFound(); // Return 404 if the BookLanguages set is null
            }
            var bookLanguage = await _context.BookLanguages.FindAsync(id);
            if (bookLanguage == null)
            {
                return NotFound(); // Return 404 if the BookLanguage is not found
            }

            _context.BookLanguages.Remove(bookLanguage); // Remove the BookLanguage from the database
            await _context.SaveChangesAsync(); // Save changes to the database

            return NoContent(); // Return 204 No Content if the deletion is successful
        }

        // Check if a BookLanguage exists by ID
        private bool BookLanguageExists(int id)
        {
            return (_context.BookLanguages?.Any(e => e.BooksId == id)).GetValueOrDefault(); // Check for the existence of the BookLanguage
        }
    }
}
