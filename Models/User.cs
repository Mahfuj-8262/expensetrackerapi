namespace ExpenseTracker.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string Email { get; set; }
        public string? PassHash { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }

    public class UserRegDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }

    public class LogInDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class RefreshDto
    {
        public required string refreshToken { get; set; }
    }

    public class AuthResponseDto
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}