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
    public class LocationsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public LocationsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Locations
        [Authorize(Roles = "Worker, Member, Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            // Check if the Locations DbSet is null
            if (_context.Locations == null)
            {
                return NotFound();
            }
            // Return the list of all locations
            return await _context.Locations.ToListAsync();
        }

        // GET: api/Locations/5
        [Authorize(Roles = "Worker, Member, Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Location>> GetLocation(string id)
        {
            // Check if the Locations DbSet is null
            if (_context.Locations == null)
            {
                return NotFound();
            }
            // Find the location by ID
            var location = await _context.Locations.FindAsync(id);

            // Return 404 if location not found
            if (location == null)
            {
                return NotFound();
            }

            return location;
        }

        // POST: api/Locations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker, Admin")]
        [HttpPost]
        public async Task<ActionResult<Location>> PostLocation(Location location)
        {
            // Check if the Locations DbSet is null
            if (_context.Locations == null)
            {
                return Problem("Entity set 'ApplicationContext.Locations' is null.");
            }
            // Add the new location to the DbSet
            _context.Locations.Add(location);
            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Handle database update exceptions
                if (LocationExists(location.Shelf))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // Return a 201 Created response with the new location
            return CreatedAtAction("GetLocation", new { id = location.Shelf }, location);
        }

        // DELETE: api/Locations/5
        [Authorize(Roles = "Worker, Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(string id)
        {
            // Check if the Locations DbSet is null
            if (_context.Locations == null)
            {
                return NotFound();
            }
            // Find the location by ID
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            // Remove the location and save changes
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper method to check if a location exists by ID
        private bool LocationExists(string id)
        {
            return (_context.Locations?.Any(e => e.Shelf == id)).GetValueOrDefault();
        }
    }
}
