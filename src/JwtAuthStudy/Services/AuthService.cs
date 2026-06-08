using JwtAuthStudy.Models.Entities;
using JwtAuthStudy.Models.Settings;
using JwtAuthStudy.Models.ViewModels;
using JwtAuthStudy.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace JwtAuthStudy.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsuarioAsync(request.Usuario, cancellationToken);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Senha, user.SenhaHash))
            return null;

        return await CreateTokenPairAsync(user, cancellationToken);
    }

    public async Task<LoginResponse?> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (storedToken is null || storedToken.Revoked || storedToken.ExpiresAt <= DateTime.UtcNow)
            return null;

        await _refreshTokenRepository.RevokeAsync(storedToken, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return await CreateTokenPairAsync(storedToken.User, cancellationToken);
    }

    public async Task<bool> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (storedToken is null || storedToken.Revoked)
            return false;

        await _refreshTokenRepository.RevokeAsync(storedToken, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<LoginResponse> CreateTokenPairAsync(User user, CancellationToken cancellationToken)
    {
        var (accessToken, expiresIn) = _jwtTokenGenerator.GenerateAccessToken(user);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = _jwtTokenGenerator.GenerateRefreshToken(),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            Revoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new LoginResponse(accessToken, refreshToken.Token, expiresIn);
    }
}
