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
using Microsoft.AspNetCore.Identity;

namespace LibraryAPI6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoansController(ApplicationContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Loans
        [Authorize(Roles = "Worker, Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Loan>>> GetLoans()
        {
            // Check if the Loans DbSet is null
            if (_context.Loans == null)
            {
                return NotFound();
            }
            // Return the list of all loans
            return await _context.Loans.ToListAsync();
        }

        // GET: api/Loans/5
        [Authorize(Roles = "Worker, Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Loan>> GetLoan(int id)
        {
            // Check if the Loans DbSet is null
            if (_context.Loans == null)
            {
                return NotFound();
            }
            // Find the loan by ID
            var loan = await _context.Loans.FindAsync(id);

            // Return 404 if loan not found
            if (loan == null)
            {
                return NotFound();
            }

            return loan;
        }

        // PUT: api/Loans/5
        [Authorize(Roles = "Worker, Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoan(int id, Loan loan)
        {
            // Check if the provided ID matches the loan ID
            if (id != loan.Id)
            {
                return BadRequest();
            }

            // Find the associated book copy
            var bookcopy = await _context.BookCopies.FindAsync(loan.BookCopyId);
            if (bookcopy == null)
            {
                return NotFound();
            }

            // Update the loan entity
            _context.Entry(loan).State = EntityState.Modified;
            bookcopy.IsAvailable = true;

            // If the loan status indicates the book is returned, update the book status
            if (loan.BookStatus)
            {
                bookcopy.BookStatus = true;
            }

            // Calculate the fine amount
            loan.FineAmount = loan.CalculateFine();
            _context.BookCopies.Update(bookcopy);

            try
            {
                // Save the changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues
                if (!LoanExists(id))
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

        // POST: api/Loans
        [Authorize(Roles = "Worker, Admin")]
        [HttpPost]
        public async Task<ActionResult<Loan>> PostLoan(Loan loan)
        {
            string username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser applicationUser = _userManager.FindByNameAsync(username).Result;
            // Set the employee ID from the current user
            loan.EmployeeId = applicationUser.Id;

            // Check if the Loans DbSet is null
            if (_context.Loans == null)
            {
                return Problem("Entity set 'ApplicationContext.Loans' is null.");
            }

            // Find the associated book copy
            var bookcopy = await _context.BookCopies!.FindAsync(loan.BookCopyId);
            if (bookcopy == null)
            {
                return NotFound();
            }

            // Check if the book copy is available
            if (!bookcopy.IsAvailable)
            {
                return BadRequest("The book copy is not available.");
            }

            // Add the loan and update the book copy status
            _context.Loans.Add(loan);
            bookcopy.IsAvailable = false;
            _context.BookCopies.Update(bookcopy);

            try
            {
                // Save the changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Handle exceptions during database update
                if (LoanExists(loan.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLoan", new { id = loan.Id }, loan);
        }

        // DELETE: api/Loans/5
        [Authorize(Roles = "Worker, Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            // Check if the Loans DbSet is null
            if (_context.Loans == null)
            {
                return NotFound();
            }

            // Find the loan by ID
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            // Remove the loan and save changes
            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper method to check if a loan exists by ID
        private bool LoanExists(int id)
        {
            return (_context.Loans?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
