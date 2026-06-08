using JwtAuthStudy.Models.Entities;

namespace JwtAuthStudy.Services.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, int ExpiresInSeconds) GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
