using JwtAuthStudy.Models.Enums;

namespace JwtAuthStudy.Models.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
