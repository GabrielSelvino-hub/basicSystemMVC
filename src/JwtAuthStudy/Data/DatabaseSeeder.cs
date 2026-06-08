using JwtAuthStudy.Models.Entities;
using JwtAuthStudy.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthStudy.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Users.AnyAsync(cancellationToken))
            return;

        context.Users.AddRange(
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Usuario = "admin",
                SenhaHash = "$2a$11$/YkIbwDn7vksmfpRfxFdyueP6m78EjN9/iHvaDuiv1kif2XSIhWpW",
                Email = "admin@jwtauthstudy.local",
                Role = UserRole.Admin
            },
            new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Usuario = "user",
                SenhaHash = "$2a$11$pzon1I8DySkDMkeUX9p5rOg.2GtjxGRRi4akK0CCbaadukVHPUjMW",
                Email = "user@jwtauthstudy.local",
                Role = UserRole.User
            });

        await context.SaveChangesAsync(cancellationToken);
    }
}
