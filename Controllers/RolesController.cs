using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryAPI6.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        // RoleManager service for managing roles
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // POST: api/Roles
        [HttpPost]
        public void CreateRoles()
        {
            // Create a new role with the name "Member"
            IdentityRole identityRole = new IdentityRole("Member");
            // Asynchronously create the "Member" role
            _roleManager.CreateAsync(identityRole).Wait();

            // Create a new role with the name "Worker"
            identityRole = new IdentityRole("Worker");
            // Asynchronously create the "Worker" role
            _roleManager.CreateAsync(identityRole).Wait();

            // Create a new role with the name "Admin"
            identityRole = new IdentityRole("Admin");
            // Asynchronously create the "Admin" role
            _roleManager.CreateAsync(identityRole).Wait();
        }
    }
}
