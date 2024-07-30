using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI6.Data;
using LibraryAPI6.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace LibraryAPI6.Controllers
{
    // API route: /api/Admins
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        // Constructor injection for context, user manager and sign-in manager
        public AdminsController(ApplicationContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/Admins
        // Only accessible by users with 'Admin' role
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmin()
        {
            // Check if Admin set is null
            if (_context.Admin == null)
            {
                return NotFound();
            }
            // Retrieve and return all admins
            return await _context.Admin.ToListAsync();
        }

        // GET: api/Admins/5
        // Only accessible by users with 'Admin' role
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(string id)
        {
            // Get current logged in user
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);

            // Check if Admin set is null
            if (_context.Admin == null)
            {
                return Unauthorized("Failed Login");
            }

            // Find admin by id
            var admin = await _context.Admin.FindAsync(id);

            // If not found, return 404
            if (admin == null)
            {
                return NotFound();
            }

            return admin;
        }

        // PUT: api/Admins/5
        // Update admin information
        // Only accessible by users with 'Admin' role
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdmin(string id, Admin admin, string? currentPassword = null)
        {
            // Find the application user by id
            ApplicationUser applicationUser = _userManager.FindByIdAsync(admin.Id).Result;

            // Check if the provided id matches the admin id
            if (id != admin.Id)
            {
                return BadRequest();
            }

            // Update user details
            _userManager.UpdateAsync(applicationUser).Wait();

            // If current password is provided, change the password
            if (currentPassword != null)
            {
                _userManager.ChangePasswordAsync(applicationUser, currentPassword, applicationUser.Password).Wait();
            }

            // Set ApplicationUser to null to avoid circular reference
            admin.ApplicationUser = null;

            // Mark entity as modified
            _context.Entry(admin).State = EntityState.Modified;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // If admin does not exist, return 404
                if (!AdminExists(id))
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

        // POST: api/Admins
        // Create a new admin
        // Only accessible by users with 'Admin' role
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Admin>> PostAdmin(Admin admin)
        {
            // Check if Admin set is null
            if (_context.Admin == null)
            {
                return Problem("Entity set 'ApplicationContext.Admin' is null.");
            }

            // Create a new user and add them to 'Admin' role
            _userManager.CreateAsync(admin.ApplicationUser!, admin.ApplicationUser!.Password).Wait();
            _userManager.AddToRoleAsync(admin.ApplicationUser, "Admin").Wait();

            // Set admin id and nullify ApplicationUser to avoid circular reference
            admin.Id = admin.ApplicationUser!.Id;
            admin.ApplicationUser = null;

            // Add admin to context
            _context.Admin.Add(admin);

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // If admin already exists, return conflict
                if (AdminExists(admin.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // Return the newly created admin
            return CreatedAtAction("GetAdmin", new { id = admin.Id }, admin);
        }

        // Helper method to check if an admin exists by id
        private bool AdminExists(string id)
        {
            return (_context.Admin?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
