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
    public class BookSubCategoriesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BookSubCategoriesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/BookSubCategories
        // Retrieve all BookSubCategory records
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookSubCategory>>> GetBookSubCategories()
        {
            if (_context.BookSubCategories == null)
            {
                return NotFound(); // Return 404 if the BookSubCategories set is null
            }
            // Retrieve and return the list of BookSubCategories
            return await _context.BookSubCategories.ToListAsync();
        }

        // GET: api/BookSubCategories/5
        // Retrieve a specific BookSubCategory by its ID
        [HttpGet("{id}")]
        public async Task<ActionResult<BookSubCategory>> GetBookSubCategory(short id)
        {
            if (_context.BookSubCategories == null)
            {
                return NotFound(); // Return 404 if the BookSubCategories set is null
            }
            var bookSubCategory = await _context.BookSubCategories.FindAsync(id);

            if (bookSubCategory == null)
            {
                return NotFound(); // Return 404 if the BookSubCategory is not found
            }

            return bookSubCategory; // Return the found BookSubCategory
        }

        // PUT: api/BookSubCategories/5
        // Update a specific BookSubCategory by its ID
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker , Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookSubCategory(short id, BookSubCategory bookSubCategory)
        {
            if (id != bookSubCategory.Id)
            {
                return BadRequest(); // Return 400 if the ID in the URL does not match the ID in the body
            }

            _context.Entry(bookSubCategory).State = EntityState.Modified; // Mark the entity as modified

            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookSubCategoryExists(id))
                {
                    return NotFound(); // Return 404 if the BookSubCategory does not exist
                }
                else
                {
                    throw; // Rethrow the exception if it's not a concurrency issue
                }
            }

            return NoContent(); // Return 204 No Content if the update is successful
        }

        // POST: api/BookSubCategories
        // Create a new BookSubCategory record
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker , Admin")]
        [HttpPost]
        public async Task<ActionResult<BookSubCategory>> PostBookSubCategory(BookSubCategory bookSubCategory)
        {
            if (_context.BookSubCategories == null)
            {
                return Problem("Entity set 'ApplicationContext.BookSubCategories' is null."); // Return 500 if the BookSubCategories set is null
            }
            _context.BookSubCategories.Add(bookSubCategory); // Add the new BookSubCategory to the database
            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database
            }
            catch (DbUpdateException)
            {
                if (BookSubCategoryExists(bookSubCategory.Id))
                {
                    return Conflict(); // Return 409 Conflict if the BookSubCategory already exists
                }
                else
                {
                    throw; // Rethrow the exception if it's not a concurrency issue
                }
            }

            return CreatedAtAction("GetBookSubCategory", new { id = bookSubCategory.Id }, bookSubCategory); // Return 201 Created with the location of the new resource
        }

        // DELETE: api/BookSubCategories/5
        // Delete a specific BookSubCategory by its ID
        [Authorize(Roles = "Worker , Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookSubCategory(short id)
        {
            if (_context.BookSubCategories == null)
            {
                return NotFound(); // Return 404 if the BookSubCategories set is null
            }
            var bookSubCategory = await _context.BookSubCategories.FindAsync(id);
            if (bookSubCategory == null)
            {
                return NotFound(); // Return 404 if the BookSubCategory is not found
            }

            _context.BookSubCategories.Remove(bookSubCategory); // Remove the BookSubCategory from the database
            await _context.SaveChangesAsync(); // Save changes to the database

            return NoContent(); // Return 204 No Content if the deletion is successful
        }

        // Check if a BookSubCategory exists by its ID
        private bool BookSubCategoryExists(short id)
        {
            return (_context.BookSubCategories?.Any(e => e.Id == id)).GetValueOrDefault(); // Check for the existence of the BookSubCategory
        }
    }
}
