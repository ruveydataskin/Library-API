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
using LibraryAPI6.Services;

namespace LibraryAPI6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public EmployeesController(
            ApplicationContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ITokenService tokenService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        // GET: api/Employees
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            if (_context.Employees == null)
            {
                return NotFound();
            }
            // Retrieve all employees with their associated ApplicationUser
            return await _context.Employees.Include(m => m.ApplicationUser).ToListAsync();
        }

        // GET: api/Employees/5
        [Authorize(Roles = "Admin , Worker")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(string id)
        {
            // Ensure the current user is authorized to access this resource
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser.Id != id)
            {
                return Unauthorized("Failed Login");
            }

            if (_context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        [Authorize(Roles = "Admin , Worker")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(string id, Employee employee, string? currentPassword = null)
        {
            if (id != employee.Id)
            {
                return BadRequest(); // Return 400 if the ID in the URL does not match the ID in the body
            }

            var applicationUser = await _userManager.FindByIdAsync(employee.Id);

            if (applicationUser == null)
            {
                return NotFound(); // Return 404 if the ApplicationUser does not exist
            }

            // Update ApplicationUser properties
            applicationUser.Address = employee.ApplicationUser!.Address;
            applicationUser.BirthDate = employee.ApplicationUser!.BirthDate;
            applicationUser.Email = employee.ApplicationUser!.Email;
            applicationUser.FamilyName = employee.ApplicationUser!.FamilyName;
            applicationUser.IdNumber = employee.ApplicationUser!.IdNumber;
            applicationUser.MiddleName = employee.ApplicationUser!.MiddleName;
            applicationUser.Name = employee.ApplicationUser!.Name;
            applicationUser.PhoneNumber = employee.ApplicationUser!.PhoneNumber;
            applicationUser.RegisterDate = employee.ApplicationUser!.RegisterDate;
            applicationUser.Status = employee.ApplicationUser!.Status;
            applicationUser.UserName = employee.ApplicationUser!.UserName;

            var updateResult = await _userManager.UpdateAsync(applicationUser);
            if (!updateResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating user");
            }

            if (currentPassword != null)
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(applicationUser, currentPassword, employee.ApplicationUser.Password);
                if (!changePasswordResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error changing password");
                }
            }

            employee.ApplicationUser = null;
            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Return 204 No Content if the update is successful
        }

        // POST: api/Employees
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'ApplicationContext.Employees' is null."); // Return 500 if the Employees set is null
            }

            var result = await _userManager.CreateAsync(employee.ApplicationUser!, employee.ApplicationUser!.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating user");
            }

            employee.Id = employee.ApplicationUser!.Id;
            employee.ApplicationUser = null;
            _context.Employees.Add(employee);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EmployeeExists(employee.Id))
                {
                    return Conflict(); // Return 409 Conflict if the employee already exists
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee); // Return 201 Created with the location of the new resource
        }

        // DELETE: api/Employees/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            if (_context.Employees == null)
            {
                return NotFound(); // Return 404 if the Employees set is null
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(); // Return 404 if the employee is not found
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent(); // Return 204 No Content if the deletion is successful
        }

        // Check if an Employee exists by its ID
        private bool EmployeeExists(string id)
        {
            return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault(); // Check for the existence of the Employee
        }
    }
}
