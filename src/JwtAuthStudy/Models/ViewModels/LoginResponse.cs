namespace JwtAuthStudy.Models.ViewModels;

public record LoginResponse(string AccessToken, string RefreshToken, int ExpiresIn);
