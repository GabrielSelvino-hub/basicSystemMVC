using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthStudy.Controllers.Api;

[ApiController]
[Route("api")]
[Produces("application/json")]
public class ProtectedApiController : ControllerBase
{
    /// <summary>Endpoint protegido — qualquer usuário autenticado.</summary>
    [HttpGet("protegido")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Protegido()
    {
        var name = User.Identity?.Name
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Name)
            ?? "usuário";

        return Ok(new { mensagem = $"Acesso autorizado, {name}!" });
    }

    /// <summary>Endpoint admin — requer role Admin.</summary>
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult Admin()
    {
        var name = User.Identity?.Name
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Name)
            ?? "admin";

        return Ok(new { mensagem = $"Painel administrativo — bem-vindo, {name}!" });
    }
}
