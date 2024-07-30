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
using Microsoft.AspNetCore.Identity;

namespace LibraryAPI6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RatingsController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Ratings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRatings()
        {
            // Check if the Ratings DbSet is null
            if (_context.Ratings == null)
            {
                return NotFound();
            }
            // Return the list of all ratings
            return await _context.Ratings.ToListAsync();
        }

        // GET: api/Ratings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rating>> GetRating(int id)
        {
            // Retrieve the current user
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);

            // Check if the Ratings DbSet is null
            if (_context.Ratings == null)
            {
                return NotFound();
            }
            // Find the rating by ID
            var rating = await _context.Ratings.FindAsync(id);

            // Return 404 if the rating is not found
            if (rating == null)
            {
                return NotFound();
            }

            return rating;
        }

        // PUT: api/Ratings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRating(int id, Rating rating)
        {
            // Check if the ID in the URL matches the ID in the request body
            if (id != rating.Id)
            {
                return BadRequest();
            }

            // Mark the rating entity as modified
            _context.Entry(rating).State = EntityState.Modified;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues
                if (!RatingExists(id))
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

        // POST: api/Ratings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin , Worker , Member")]
        [HttpPost]
        public IActionResult RateBook(int bookId, string memberId, int score)
        {
            // Validate the rating score
            if (score < 1 || score > 5)
            {
                return BadRequest("Rating must be between 1 and 5.");
            }

            // Check if the member has borrowed and returned the book
            var loan = _context.Loans!.FirstOrDefault(l => l.BookCopy!.BookId == bookId && l.MemberId == memberId && l.ReturnedDate != null);
            if (loan == null)
            {
                return BadRequest("You have not borrowed and returned this book.");
            }

            // Check if the member has already rated this book
            var existingRating = _context.Ratings!.FirstOrDefault(r => r.BookId == bookId && r.MemberId == memberId);
            if (existingRating != null)
            {
                return BadRequest("You have already rated this book.");
            }

            // Create a new rating
            var rating = new Rating
            {
                BookId = bookId,
                MemberId = memberId,
                Score = score
            };

            // Update the book's rating
            var book = _context.Books!.FirstOrDefault(b => b.Id == rating.BookId);
            if (book != null)
            {
                book.VoteCount++;
                book.VoteSum += rating.Score;
                book.Rating = book.VoteSum / book.VoteCount;

                _context.Books!.Update(book);
            }

            // Add the new rating to the database
            _context.Ratings!.Add(rating);
            _context.SaveChanges();

            return Ok("Book rated successfully.");
        }

        // DELETE: api/Ratings/5
        [Authorize(Roles = "Admin , Worker")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            // Check if the Ratings DbSet is null
            if (_context.Ratings == null)
            {
                return NotFound();
            }
            // Find the rating by ID
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            // Remove the rating and save changes
            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper method to check if a rating exists by ID
        private bool RatingExists(int id)
        {
            return (_context.Ratings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
