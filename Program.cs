using System.Security.Claims;
using System.Text;
using LibraryAPI6.Data;
using LibraryAPI6.Models;
using LibraryAPI6.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace LibraryAPI6
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Declare necessary variables
            ApplicationContext _context;
            RoleManager<IdentityRole> _roleManager;
            UserManager<ApplicationUser> _userManager;
            IdentityRole identityRole;
            ApplicationUser applicationUser;
            Language language;

            var builder = WebApplication.CreateBuilder(args);

            // Add database context to the service container with SQL Server
            builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationContext")));

            // Configure Identity services
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders();

            // Configure JWT authentication
            var jwtSettings = builder.Configuration.GetSection("Jwt");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero, // Set clock skew to zero for token expiration
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                    RoleClaimType = ClaimTypes.Role
                };
            });

            // Register custom services
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddAuthorization();

            // Add MVC controllers
            builder.Services.AddControllers();

            // Configure Swagger for API documentation and JWT authentication
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Create a scope to resolve the services and perform database setup
            _context = app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationContext>();
            _roleManager = app.Services.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = app.Services.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Apply pending migrations to the database
            _context.Database.Migrate();

            // Create the Admin role if it does not exist
            if (_roleManager.FindByIdAsync("Admin").Result == null)
            {
                identityRole = new IdentityRole("Admin");
                _roleManager.CreateAsync(identityRole).Wait();
            }

            // Create an Admin user if it does not exist and assign the Admin role
            if (_userManager.FindByIdAsync("Admin").Result == null)
            {
                applicationUser = new ApplicationUser
                {
                    UserName = "Admin"
                };
                _userManager.CreateAsync(applicationUser, "Admin123!").Wait();
                _userManager.AddToRoleAsync(applicationUser, "Admin").Wait();
            }

            // Add a default language entry if it does not exist
            if (_context.Languages!.Find("tur") == null)
            {
                language = new Language
                {
                    Code = "tur",
                    Name = "Türkçe"
                };
                _context.Languages!.Add(language);
                _context.SaveChanges();
            }

            app.Run();
        }
    }
}
