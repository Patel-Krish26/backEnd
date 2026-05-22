namespace backEnd.DTOs;

public class RegisterDto
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; } // "admin", "farmer", "customer"
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? BirthDate { get; set; }
    public string? Bio { get; set; }
    public string? Avatar { get; set; }
}

public class LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class AuthResponseDto
{
    public required string Token { get; set; }
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public required string Role { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string JoinedDate { get; set; } = string.Empty;
}
