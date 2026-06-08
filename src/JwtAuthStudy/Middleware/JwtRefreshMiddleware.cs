using System.IdentityModel.Tokens.Jwt;
using JwtAuthStudy.Helpers;
using JwtAuthStudy.Models.ViewModels;
using JwtAuthStudy.Services.Interfaces;

namespace JwtAuthStudy.Middleware;

public class JwtRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;

    public JwtRefreshMiddleware(RequestDelegate next, IWebHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        var accessToken = context.Request.Cookies[AuthCookieHelper.AccessToken];
        var refreshToken = context.Request.Cookies[AuthCookieHelper.RefreshToken];

        if (!string.IsNullOrEmpty(refreshToken) && ShouldRefresh(accessToken))
        {
            var result = await authService.RefreshAsync(new RefreshTokenRequest(refreshToken));
            if (result is not null)
            {
                AuthCookieHelper.SetAuthCookies(context.Response, result, _environment.IsDevelopment());
                context.Items[AuthCookieHelper.AccessToken] = result.AccessToken;
            }
        }

        await _next(context);
    }

    private static bool ShouldRefresh(string? accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
            return true;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);
            return jwt.ValidTo <= DateTime.UtcNow.AddMinutes(1);
        }
        catch
        {
            return true;
        }
    }
}
