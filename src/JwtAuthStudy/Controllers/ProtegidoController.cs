using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtAuthStudy.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthStudy.Controllers;

[Authorize]
public class ProtegidoController : Controller
{
    public IActionResult Index()
    {
        var model = new ProtegidoViewModel
        {
            Usuario = User.Identity?.Name
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Name)
                ?? "usuário",
            Email = User.FindFirstValue(JwtRegisteredClaimNames.Email) ?? "-",
            Role = User.FindFirstValue(ClaimTypes.Role) ?? "-"
        };

        return View(model);
    }
}
