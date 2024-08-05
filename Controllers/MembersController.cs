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
using System.Security.Claims;

namespace LibraryAPI6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public MembersController(ApplicationContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/Members
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            // Check if the Members DbSet is null
            if (_context.Members == null)
            {
                return NotFound();
            }
            // Return the list of all members including their ApplicationUser details
            return await _context.Members.Include(m => m.ApplicationUser).ToListAsync();
        }

        // GET: api/Members/5
        [Authorize(Roles = "Admin , Member")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(string id)
        {
            string username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser applicationUser = _userManager.FindByNameAsync(username).Result;
            // Ensure the current user is authorized to access this resource
            if (applicationUser == null || applicationUser.Id != id)
            {
                return Unauthorized();
            }

            if (_context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // PUT: api/Members/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin, Worker")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(string id, Member member, string? currentPassword = null)
        {

            if (User.IsInRole("Admin") == false)
            {
                string username = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ApplicationUser applicationUser1 = _userManager.FindByNameAsync(username).Result;

                // Ensure the current user is authorized to access this resource
                if (applicationUser1 == null || applicationUser1.Id != id)
                {
                    return Unauthorized();
                }
            }

            // Retrieve the ApplicationUser by ID
            ApplicationUser applicationUser = _userManager.FindByIdAsync(member.Id).Result;

            // Check if the ID in the URL matches the ID in the request body
            if (applicationUser == null)
            {
                return NotFound(); // Return 404 if the ApplicationUser does not exist
            }

            // Update ApplicationUser properties
            applicationUser.Address = member.ApplicationUser!.Address;
            applicationUser.BirthDate = member.ApplicationUser!.BirthDate;
            applicationUser.Email = member.ApplicationUser!.Email;
            applicationUser.FamilyName = member.ApplicationUser!.FamilyName;
            applicationUser.Id = member.ApplicationUser!.Id;
            applicationUser.IdNumber = member.ApplicationUser!.IdNumber;
            applicationUser.MiddleName = member.ApplicationUser!.MiddleName;
            applicationUser.Name = member.ApplicationUser!.Name;
            applicationUser.Password = member.ApplicationUser!.Password;
            applicationUser.PhoneNumber = member.ApplicationUser!.PhoneNumber;
            applicationUser.RegisterDate = member.ApplicationUser!.RegisterDate;
            applicationUser.Status = member.ApplicationUser!.Status;
            applicationUser.UserName = member.ApplicationUser!.UserName;

            // Update the ApplicationUser asynchronously
            _userManager.UpdateAsync(applicationUser).Wait();

            // Change the password if currentPassword is provided
            if (currentPassword != null)
            {
                _userManager.ChangePasswordAsync(applicationUser, currentPassword, applicationUser.Password).Wait();
            }

            // Remove ApplicationUser from member before saving
            member.ApplicationUser = null;

            // Mark the member entity as modified
            _context.Entry(member).State = EntityState.Modified;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues
                if (!MemberExists(id))
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

        // POST: api/Members
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Worker, Admin")]
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
            // Check if the Members DbSet is null
            if (_context.Members == null)
            {
                return Problem("Entity set 'ApplicationContext.Members' is null.");
            }
            // Create the ApplicationUser asynchronously
            _userManager.CreateAsync(member.ApplicationUser!, member.ApplicationUser!.Password).Wait();
            _userManager.AddToRoleAsync(member.ApplicationUser, "Member").Wait();
            member.Id = member.ApplicationUser!.Id;
            member.ApplicationUser = null;
            _context.Members.Add(member);

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Handle database update exceptions
                if (MemberExists(member.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // Return a 201 Created response with the new member
            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }

        // DELETE: api/Members/5
        [Authorize(Roles = "Worker, Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(string id)
        {
            // Find the member by ID
            var member = await _userManager.FindByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            member.Status = 0;
            // Remove the member and save changes
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper method to check if a member exists by ID
        private bool MemberExists(string id)
        {
            return (_context.Members?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
