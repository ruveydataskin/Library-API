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
    public class PublishersController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PublishersController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Publishers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publisher>>> GetPublishers()
        {
            // Check if the Publishers DbSet is null
            if (_context.Publishers == null)
            {
                return NotFound();
            }
            // Return the list of all publishers
            return await _context.Publishers.ToListAsync();
        }

        // GET: api/Publishers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Publisher>> GetPublisher(int id)
        {
            // Check if the Publishers DbSet is null
            if (_context.Publishers == null)
            {
                return NotFound();
            }
            // Find the publisher by ID
            var publisher = await _context.Publishers.FindAsync(id);

            // Return 404 if publisher not found
            if (publisher == null)
            {
                return NotFound();
            }

            return publisher;
        }

        // PUT: api/Publishers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker , Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublisher(int id, Publisher publisher)
        {
            // Check if the ID in the URL matches the ID in the request body
            if (id != publisher.Id)
            {
                return BadRequest();
            }

            // Mark the publisher entity as modified
            _context.Entry(publisher).State = EntityState.Modified;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues
                if (!PublisherExists(id))
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

        // POST: api/Publishers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker , Admin")]
        [HttpPost]
        public async Task<ActionResult<Publisher>> PostPublisher(Publisher publisher)
        {
            // Check if the Publishers DbSet is null
            if (_context.Publishers == null)
            {
                return Problem("Entity set 'ApplicationContext.Publishers' is null.");
            }
            // Add the new publisher to the database
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            // Return a 201 Created response with the new publisher
            return CreatedAtAction("GetPublisher", new { id = publisher.Id }, publisher);
        }

        // DELETE: api/Publishers/5
        [Authorize(Roles = "Worker , Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            // Check if the Publishers DbSet is null
            if (_context.Publishers == null)
            {
                return NotFound();
            }
            // Find the publisher by ID
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }

            // Remove the publisher and save changes
            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper method to check if a publisher exists by ID
        private bool PublisherExists(int id)
        {
            return (_context.Publishers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
