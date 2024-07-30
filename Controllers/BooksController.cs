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
using System.Security.Claims;

namespace LibraryAPI6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BooksController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Books
        // Retrieve all Book records including their related BookSubCategories and SubCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            if (_context.Books == null)
            {
                return NotFound(); // Return 404 if the Books set is null
            }
            // Retrieve and return the list of Books with their associated BookSubCategories and SubCategories
            return await _context.Books.Include(m => m.BookSubCategories)!.ThenInclude(m => m.SubCategory).ToListAsync();
        }

        // GET: api/Books/5
        // Retrieve a specific Book by its ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            if (_context.Books == null)
            {
                return NotFound(); // Return 404 if the Books set is null
            }
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound(); // Return 404 if the Book is not found
            }

            return book; // Return the found Book
        }

        // PUT: api/Books/5
        // Update a specific Book by its ID
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker , Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest(); // Return 400 if the ID in the URL does not match the ID in the body
            }

            _context.Entry(book).State = EntityState.Modified; // Mark the entity as modified

            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound(); // Return 404 if the Book does not exist
                }
                else
                {
                    throw; // Rethrow the exception if it's not a concurrency issue
                }
            }

            return NoContent(); // Return 204 No Content if the update is successful
        }

        // POST: api/Books
        // Create a new Book record
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker , Admin")]
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            // Retrieve the current user's ID (not used in this method but included for reference)
            User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (_context.Books == null)
            {
                return Problem("Entity set 'ApplicationContext.Books' is null."); // Return 500 if the Books set is null
            }
            _context.Books.Add(book); // Add the new Book to the database
            await _context.SaveChangesAsync(); // Save changes to the database

            return CreatedAtAction("GetBook", new { id = book.Id }, book); // Return 201 Created with the location of the new resource
        }

        // DELETE: api/Books/5
        // Delete a specific Book by its ID
        [Authorize(Roles = "Worker , Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (_context.Books == null)
            {
                return NotFound(); // Return 404 if the Books set is null
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(); // Return 404 if the Book is not found
            }

            _context.Books.Remove(book); // Remove the Book from the database
            await _context.SaveChangesAsync(); // Save changes to the database

            return NoContent(); // Return 204 No Content if the deletion is successful
        }

        // Check if a Book exists by its ID
        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault(); // Check for the existence of the Book
        }
    }
}
