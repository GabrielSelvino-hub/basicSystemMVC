using JwtAuthStudy.Models.ViewModels;

namespace JwtAuthStudy.Helpers;

public static class AuthCookieHelper
{
    public const string AccessToken = "access_token";
    public const string RefreshToken = "refresh_token";

    public static void SetAuthCookies(HttpResponse response, LoginResponse login, bool isDevelopment)
    {
        var options = CreateCookieOptions(isDevelopment);

        response.Cookies.Append(AccessToken, login.AccessToken, options);
        response.Cookies.Append(RefreshToken, login.RefreshToken, options);
    }

    public static void ClearAuthCookies(HttpResponse response)
    {
        response.Cookies.Delete(AccessToken);
        response.Cookies.Delete(RefreshToken);
    }

    public static CookieOptions CreateCookieOptions(bool isDevelopment) => new()
    {
        HttpOnly = true,
        Secure = !isDevelopment,
        SameSite = SameSiteMode.Lax,
        IsEssential = true
    };
}
