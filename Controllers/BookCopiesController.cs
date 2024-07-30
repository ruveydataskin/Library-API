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
    public class BookCopiesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BookCopiesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/BookCopies
        // Retrieve all BookCopy records
        [Authorize(Roles = "Worker , Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookCopy>>> GetBookCopies()
        {
            if (_context.BookCopies == null)
            {
                return NotFound(); // Return 404 if the BookCopies set is null
            }
            return await _context.BookCopies.ToListAsync(); // Retrieve and return the list of BookCopies
        }

        // GET: api/BookCopies/5
        // Retrieve a specific BookCopy by ID
        [Authorize(Roles = "Worker , Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<BookCopy>> GetBookCopy(int id)
        {
            if (_context.BookCopies == null)
            {
                return NotFound(); // Return 404 if the BookCopies set is null
            }
            var bookCopy = await _context.BookCopies.FindAsync(id);

            if (bookCopy == null)
            {
                return NotFound(); // Return 404 if the BookCopy is not found
            }

            return bookCopy; // Return the found BookCopy
        }

        // PUT: api/BookCopies/5
        // Update a specific BookCopy by ID
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker , Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookCopy(int id, BookCopy bookCopy)
        {
            if (id != bookCopy.Id)
            {
                return BadRequest(); // Return 400 if the ID in the URL does not match the ID in the body
            }

            _context.Entry(bookCopy).State = EntityState.Modified; // Mark the entity as modified

            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookCopyExists(id))
                {
                    return NotFound(); // Return 404 if the BookCopy does not exist
                }
                else
                {
                    throw; // Rethrow the exception if it's not a concurrency issue
                }
            }

            return NoContent(); // Return 204 No Content if the update is successful
        }

        // POST: api/BookCopies
        // Create a new BookCopy record
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker , Admin")]
        [HttpPost]
        public async Task<ActionResult<BookCopy>> PostBookCopy(BookCopy bookCopy)
        {
            if (_context.BookCopies == null)
            {
                return Problem("Entity set 'ApplicationContext.BookCopies' is null."); // Return 500 if the BookCopies set is null
            }
            bookCopy.IsAvailable = true; // Set the default availability of the BookCopy to true
            _context.BookCopies.Add(bookCopy); // Add the new BookCopy to the database
            await _context.SaveChangesAsync(); // Save changes to the database

            return CreatedAtAction("GetBookCopy", new { id = bookCopy.Id }, bookCopy); // Return 201 Created with the location of the new resource
        }

        // DELETE: api/BookCopies/5
        // Delete a specific BookCopy by ID
        [Authorize(Roles = "Worker , Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookCopy(int id)
        {
            if (_context.BookCopies == null)
            {
                return NotFound(); // Return 404 if the BookCopies set is null
            }
            var bookCopy = await _context.BookCopies.FindAsync(id);
            if (bookCopy == null)
            {
                return NotFound(); // Return 404 if the BookCopy is not found
            }

            _context.BookCopies.Remove(bookCopy); // Remove the BookCopy from the database
            await _context.SaveChangesAsync(); // Save changes to the database

            return NoContent(); // Return 204 No Content if the deletion is successful
        }

        // Check if a BookCopy exists by ID
        private bool BookCopyExists(int id)
        {
            return (_context.BookCopies?.Any(e => e.Id == id)).GetValueOrDefault(); // Check for the existence of the BookCopy
        }
    }
}
