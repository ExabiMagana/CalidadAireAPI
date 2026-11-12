
using CalidadAireAPI.Data;
using CalidadAireAPI.DTOs;
using CalidadAireAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CalidadAireAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CalidadAireDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public AuthController(
            CalidadAireDbContext context,
            IConfiguration configuration,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(x => x.UsuarioNombre == dto.Usuario);

            if (usuario == null ||
                _passwordHasher.VerifyHashedPassword(
                    usuario, usuario.ContrasenaHash, dto.Contrasena
                ) == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Falta Jwt:Key.");

            var issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("Falta Jwt:Issuer.");

            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("Falta Jwt:Audience.");

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.UsuarioNombre)
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60")),
                signingCredentials: credentials
            );

            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso.",
                usuario = usuario.UsuarioNombre,
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
