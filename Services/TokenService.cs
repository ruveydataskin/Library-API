using LibraryAPI6.Models;
using LibraryAPI6.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    // Constructor to initialize UserManager and IConfiguration
    public TokenService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    // Method to generate JWT token for a given user
    public async Task<string> GenerateToken(ApplicationUser user)
    {
        // Retrieve the user's claims (additional information about the user)
        var userClaims = await _userManager.GetClaimsAsync(user);

        // Retrieve the user's roles and create claims for them
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        // Create a list of claims to be included in the token
        var claims = new List<Claim>
        {
            // User's username (subject of the token)
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),

            // Unique identifier for the token (JWT ID)
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // User's unique identifier (user ID)
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        // Add user's email if it exists
        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        }

        // Add user's claims and roles to the list of claims
        claims.AddRange(userClaims);
        claims.AddRange(roleClaims);

        // Create the key for signing the token from configuration
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Define the expiration time of the token from configuration
        var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]));

        // Create the JWT token with issuer, audience, claims, expiration, and signing credentials
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds);

        // Return the serialized token
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
