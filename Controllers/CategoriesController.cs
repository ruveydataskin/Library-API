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
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public CategoriesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        // Retrieve all Category records, including their SubCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            if (_context.Categories == null)
            {
                return NotFound(); // Return 404 if the Categories set is null
            }
            // Retrieve and return the list of Categories including their SubCategories
            return await _context.Categories.Include(c => c.SubCategories).ToListAsync();
        }

        // GET: api/Categories/5
        // Retrieve a specific Category by its ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(short id)
        {
            if (_context.Categories == null)
            {
                return NotFound(); // Return 404 if the Categories set is null
            }
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(); // Return 404 if the Category is not found
            }

            return category; // Return the found Category
        }

        // PUT: api/Categories/5
        // Update a specific Category by its ID
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker , Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(short id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest(); // Return 400 if the ID in the URL does not match the ID in the body
            }

            _context.Entry(category).State = EntityState.Modified; // Mark the entity as modified

            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound(); // Return 404 if the Category does not exist
                }
                else
                {
                    throw; // Rethrow the exception if it's not a concurrency issue
                }
            }

            return NoContent(); // Return 204 No Content if the update is successful
        }

        // POST: api/Categories
        // Create a new Category record
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker , Admin")]
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve the current user's ID
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationContext.Categories' is null."); // Return 500 if the Categories set is null
            }
            _context.Categories.Add(category); // Add the new Category to the database
            await _context.SaveChangesAsync(); // Save changes to the database

            return CreatedAtAction("GetCategory", new { id = category.Id }, category); // Return 201 Created with the location of the new resource
        }

        // DELETE: api/Categories/5
        // Delete a specific Category by its ID
        [Authorize(Roles = "Worker , Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(short id)
        {
            if (_context.Categories == null)
            {
                return NotFound(); // Return 404 if the Categories set is null
            }
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(); // Return 404 if the Category is not found
            }

            _context.Categories.Remove(category); // Remove the Category from the database
            await _context.SaveChangesAsync(); // Save changes to the database

            return NoContent(); // Return 204 No Content if the deletion is successful
        }

        // Check if a Category exists by its ID
        private bool CategoryExists(short id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault(); // Check for the existence of the Category
        }
    }
}
