using JwtAuthStudy.Models.ViewModels;

namespace JwtAuthStudy.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse?> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<bool> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default);
}
