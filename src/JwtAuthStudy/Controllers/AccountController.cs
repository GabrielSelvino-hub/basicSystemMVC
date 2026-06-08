using JwtAuthStudy.Helpers;
using JwtAuthStudy.Models.ViewModels;
using JwtAuthStudy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthStudy.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _environment;

    public AccountController(IAuthService authService, IWebHostEnvironment environment)
    {
        _authService = authService;
        _environment = environment;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Protegido");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginAsync(
            new LoginRequest(model.Usuario, model.Senha),
            cancellationToken);

        if (result is null)
        {
            ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
            return View(model);
        }

        AuthCookieHelper.SetAuthCookies(Response, result, _environment.IsDevelopment());

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Protegido");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[AuthCookieHelper.RefreshToken];
        if (!string.IsNullOrEmpty(refreshToken))
            await _authService.LogoutAsync(new LogoutRequest(refreshToken), cancellationToken);

        AuthCookieHelper.ClearAuthCookies(Response);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[AuthCookieHelper.RefreshToken];
        if (string.IsNullOrEmpty(refreshToken))
            return RedirectToAction("Login");

        var result = await _authService.RefreshAsync(new RefreshTokenRequest(refreshToken), cancellationToken);
        if (result is null)
        {
            AuthCookieHelper.ClearAuthCookies(Response);
            return RedirectToAction("Login");
        }

        AuthCookieHelper.SetAuthCookies(Response, result, _environment.IsDevelopment());
        return RedirectToAction("Index", "Protegido");
    }
}
