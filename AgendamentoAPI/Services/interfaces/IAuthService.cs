using AgendamentoAPI.DTOs.Auth;
using AgendamentoAPI.Models;

namespace AgendamentoAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegistrarAsync(RegistroDto registroDto);
        Task<AuthResponse> LoginAsync(LoginDto loginDto);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task RevogarRefreshTokenAsync(string usuarioId);
        Task<UsuarioInfoResponse> ObterUsuarioPorIdAsync(string usuarioId);
    }
}