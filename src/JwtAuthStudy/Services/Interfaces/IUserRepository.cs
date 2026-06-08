using JwtAuthStudy.Models.Entities;

namespace JwtAuthStudy.Services.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsuarioAsync(string usuario, CancellationToken cancellationToken = default);
}
