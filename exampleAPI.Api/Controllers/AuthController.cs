using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MiApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // TODO: Validar credenciales contra la BD con tu MediatR / Service
        // if (!esValido) return Unauthorized("Usuario o contraseña incorrectos");

        var token = GenerarJwtToken(request.Username);

        return Ok(new 
        { 
            Token = token,
            TokenType = "Bearer"
        });
    }

    private string GenerarJwtToken(string username)
    {
        var secret = _configuration["Jwt:Secret"]!;
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        // 1. Duración dinámica (60 minutos por defecto)
        var expirationMinutes = int.TryParse(_configuration["Jwt:ExpirationInMinutes"], out var minutes) ? minutes : 60;

        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(expirationMinutes);

        var key = Encoding.UTF8.GetBytes(secret);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin"),
            
            // 2. Claim iat:  hashes únicos en cada llamada
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            
            // 3. Claim exp: marca de expiración del estándar JWT
            new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(expires).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}

public record LoginRequest(string Username, string Password);