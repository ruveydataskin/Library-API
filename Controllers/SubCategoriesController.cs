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
    public class SubCategoriesController : ControllerBase
    {
        // ApplicationContext to manage the database context
        private readonly ApplicationContext _context;

        public SubCategoriesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/SubCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubCategory>>> GetSubCategories()
        {
            // Return 404 Not Found if SubCategories collection is null
            if (_context.SubCategories == null)
            {
                return NotFound();
            }
            // Return all SubCategory records as a list
            return await _context.SubCategories.ToListAsync();
        }

        // GET: api/SubCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubCategory>> GetSubCategory(short id)
        {
            // Return 404 Not Found if SubCategories collection is null
            if (_context.SubCategories == null)
            {
                return NotFound();
            }
            // Find SubCategory by ID
            var subCategory = await _context.SubCategories.FindAsync(id);

            // Return 404 Not Found if SubCategory is not found
            if (subCategory == null)
            {
                return NotFound();
            }

            // Return the found SubCategory
            return subCategory;
        }

        // PUT: api/SubCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker , Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubCategory(short id, SubCategory subCategory)
        {
            // Return 400 Bad Request if the ID does not match
            if (id != subCategory.Id)
            {
                return BadRequest();
            }

            // Mark the entity as modified
            _context.Entry(subCategory).State = EntityState.Modified;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Return 404 Not Found if the SubCategory does not exist
                if (!SubCategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Return 204 No Content if the update is successful
            return NoContent();
        }

        // POST: api/SubCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker , Admin")]
        [HttpPost]
        public async Task<ActionResult<SubCategory>> PostSubCategory(SubCategory subCategory)
        {
            // Return a problem status if SubCategories collection is null
            if (_context.SubCategories == null)
            {
                return Problem("Entity set 'ApplicationContext.SubCategories' is null.");
            }
            // Add the new SubCategory
            _context.SubCategories.Add(subCategory);
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the newly created SubCategory
            return CreatedAtAction("GetSubCategory", new { id = subCategory.Id }, subCategory);
        }

        // DELETE: api/SubCategories/5
        [Authorize(Roles = "Worker , Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(short id)
        {
            // Return 404 Not Found if SubCategories collection is null
            if (_context.SubCategories == null)
            {
                return NotFound();
            }
            // Find SubCategory by ID
            var subCategory = await _context.SubCategories.FindAsync(id);
            // Return 404 Not Found if SubCategory is not found
            if (subCategory == null)
            {
                return NotFound();
            }

            // Remove the SubCategory
            _context.SubCategories.Remove(subCategory);
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return 204 No Content if the deletion is successful
            return NoContent();
        }

        // Helper method to check if a SubCategory with a specific ID exists
        private bool SubCategoryExists(short id)
        {
            return (_context.SubCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
