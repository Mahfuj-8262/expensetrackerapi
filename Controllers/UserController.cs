using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Data;
using ExpenseTracker.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        public UserController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email)) return BadRequest("User already exist!");

            var hasher = new PasswordHasher<User>();

            var user = new User
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            user.PassHash = hasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User Registration Successfull!");
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LogInDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return Unauthorized(new { error = "Invalid credentials" });

            var hasher = new PasswordHasher<User>();

            var result = hasher.VerifyHashedPassword(user, user.PassHash!, dto.Password);

            if (result == PasswordVerificationResult.Failed) return Unauthorized(new { error = "Invalid credentials!" });

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokenHash = HashToken(refreshToken.Token);
            user.RefreshTokenExpiryTime = refreshToken.Expires;

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshDto dto)
        {
            var hash = HashToken(dto.refreshToken);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshTokenHash == hash);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow) return Unauthorized(new { error = "Invalid refresh token!" });

            var accessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshTokenHash = HashToken(newRefreshToken.Token);
            user.RefreshTokenExpiryTime = newRefreshToken.Expires;

            await _context.SaveChangesAsync();
            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        private string GenerateJwtToken(User user)
        {
            var JwtConfig = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName)
            };

            var token = new JwtSecurityToken(
                issuer: JwtConfig["Issuer"],
                audience: JwtConfig["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(JwtConfig["ExpireMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private (string Token, DateTime Expires) GenerateRefreshToken()
        {
            var JwtConfig = _config.GetSection("Jwt");
            var rand = new byte[64];
            using var randGenerator = RandomNumberGenerator.Create();
            randGenerator.GetBytes(rand);
            return (
                Convert.ToBase64String(rand),
                DateTime.UtcNow.AddDays(int.Parse(JwtConfig["RefreshTokenExpireDays"]!))
            );
        }

        private string HashToken(string Token)
        {
            var bytes = Encoding.UTF8.GetBytes(Token);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
