using JwtAuthStudy.Data;
using JwtAuthStudy.Models.Entities;
using JwtAuthStudy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthStudy.Services;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByUsuarioAsync(string usuario, CancellationToken cancellationToken = default) =>
        _context.Users.FirstOrDefaultAsync(u => u.Usuario == usuario, cancellationToken);
}
