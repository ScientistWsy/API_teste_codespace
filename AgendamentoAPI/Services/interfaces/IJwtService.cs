using AgendamentoAPI.Models;
using System.Security.Claims;

namespace AgendamentoAPI.Services.Abstractions
{
    public interface IJwtService
    {
        string GenerateToken(Usuario usuario);
    }
}