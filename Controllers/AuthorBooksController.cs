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
    public class AuthorBooksController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public AuthorBooksController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/AuthorBooks
        // Retrieve all AuthorBook records
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorBook>>> GetAuthorBooks()
        {
            if (_context.AuthorBooks == null)
            {
                return NotFound();
            }
            return await _context.AuthorBooks.ToListAsync();
        }

        // GET: api/AuthorBooks/5
        // Retrieve a specific AuthorBook by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorBook>> GetAuthorBook(long id)
        {
            if (_context.AuthorBooks == null)
            {
                return NotFound();
            }
            var authorBook = await _context.AuthorBooks.FindAsync(id);

            if (authorBook == null)
            {
                return NotFound();
            }

            return authorBook;
        }

        // PUT: api/AuthorBooks/5
        // Update a specific AuthorBook by ID
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker, Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthorBook(long id, AuthorBook authorBook)
        {
            if (id != authorBook.AuthorsId)
            {
                return BadRequest();
            }

            _context.Entry(authorBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorBookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AuthorBooks
        // Create a new AuthorBook record
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker, Admin")]
        [HttpPost]
        public async Task<ActionResult<AuthorBook>> PostAuthorBook(AuthorBook authorBook)
        {
            if (_context.AuthorBooks == null)
            {
                return Problem("Entity set 'ApplicationContext.AuthorBooks' is null.");
            }
            _context.AuthorBooks.Add(authorBook);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AuthorBookExists(authorBook.AuthorsId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAuthorBook", new { id = authorBook.AuthorsId }, authorBook);
        }

        // DELETE: api/AuthorBooks/5
        // Delete a specific AuthorBook by ID
        [Authorize(Roles = "Worker, Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthorBook(long id)
        {
            if (_context.AuthorBooks == null)
            {
                return NotFound();
            }
            var authorBook = await _context.AuthorBooks.FindAsync(id);
            if (authorBook == null)
            {
                return NotFound();
            }

            _context.AuthorBooks.Remove(authorBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Check if an AuthorBook exists by ID
        private bool AuthorBookExists(long id)
        {
            return (_context.AuthorBooks?.Any(e => e.AuthorsId == id)).GetValueOrDefault();
        }
    }
}
