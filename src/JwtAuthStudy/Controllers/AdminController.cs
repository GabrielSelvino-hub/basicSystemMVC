using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtAuthStudy.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthStudy.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        var model = new ProtegidoViewModel
        {
            Usuario = User.Identity?.Name
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Name)
                ?? "admin",
            Email = User.FindFirstValue(JwtRegisteredClaimNames.Email) ?? "-",
            Role = "Admin"
        };

        return View(model);
    }
}
