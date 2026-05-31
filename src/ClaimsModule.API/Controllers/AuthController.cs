using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClaimsModule.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ClaimsModule.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var (userId, role, name) = request.Username switch
        {
            "supervisor" => (SeedConstants.SupervisorUserId, "supervisor", "supervisor@claims.local"),
            "manager" => (SeedConstants.ManagerUserId, "manager", "manager@claims.local"),
            _ => (SeedConstants.HandlerUserId, "handler", "handler@claims.local")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "ClaimsModuleDevSecretKey_ChangeInProduction_32chars!!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"] ?? "ClaimsModule",
            audience: configuration["Jwt:Audience"] ?? "ClaimsModule",
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role)
            },
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), role, userName = name });
    }
}

public record LoginRequest(string Username, string Password);
