﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AgendamentoAPI.Data;
using AgendamentoAPI.DTOs.Auth;
using AgendamentoAPI.Models;
using AgendamentoAPI.Services.Abstractions;

using AgendamentoAPI.Models.Configuration;
using Microsoft.Extensions.Options;
namespace AgendamentoAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            AppDbContext context,
            IJwtService jwtService,
            IPasswordHasher<Usuario> passwordHasher,
            ILogger<AuthService> logger,
            IHttpContextAccessor httpContextAccessor,
            IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponse> RegistrarAsync(RegistroDto registroDto)
        {
            try
            {
                // Verifica se email já está cadastrado
                if (await _context.Usuarios.AnyAsync(u => u.Email == registroDto.Email))
                {
                    return new AuthResponse
                    {
                        Sucesso = false,
                        Mensagem = "Email já cadastrado"
                    };
                }

                // Cria o usuário base
                var usuario = new Usuario
                {
                    Nome = registroDto.Nome,
                    Email = registroDto.Email,
                    Perfil = registroDto.Perfil
                };

                // Aplica hash na senha
                usuario.SenhaHash = _passwordHasher.HashPassword(usuario, registroDto.Senha);

                // Cria o perfil específico (Paciente ou Medico)
                if (registroDto.Perfil == "Paciente")
                {
                    var paciente = new Paciente
                    {
                        Usuario = usuario,
                        CPF = registroDto.CPF,
                        DataNascimento = registroDto.DataNascimento
                    };
                    await _context.Pacientes.AddAsync(paciente);
                }
                else if (registroDto.Perfil == "Medico")
                {
                    var medico = new Medico
                    {
                        Usuario = usuario,
                        CRM = registroDto.CRM
                    };
                    await _context.Medicos.AddAsync(medico);
                }

                await _context.SaveChangesAsync();

                // Gera os tokens
                return await GerarRespostaAutenticacao(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário");
                return new AuthResponse
                {
                    Sucesso = false,
                    Mensagem = "Ocorreu um erro ao registrar o usuário"
                };
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Paciente)
                    .Include(u => u.Medico)
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (usuario == null)
                {
                    return new AuthResponse
                    {
                        Sucesso = false,
                        Mensagem = "Credenciais inválidas"
                    };
                }

                // Verifica a senha
                var resultadoVerificacao = _passwordHasher.VerifyHashedPassword(
                    usuario, usuario.SenhaHash, loginDto.Senha);

                if (resultadoVerificacao != PasswordVerificationResult.Success)
                {
                    return new AuthResponse
                    {
                        Sucesso = false,
                        Mensagem = "Credenciais inválidas"
                    };
                }

                // Gera os tokens
                return await GerarRespostaAutenticacao(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao autenticar usuário");
                return new AuthResponse
                {
                    Sucesso = false,
                    Mensagem = "Ocorreu um erro ao autenticar"
                };
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var tokenArmazenado = await _context.RefreshTokens
                    .Include(rt => rt.Usuario)
                    .FirstOrDefaultAsync(rt =>
                        rt.Token == refreshToken &&
                        rt.ExpiresAt > DateTime.UtcNow &&
                        rt.RevokedAt == null);

                if (tokenArmazenado == null)
                {
                    return new AuthResponse
                    {
                        Sucesso = false,
                        Mensagem = "Refresh token inválido ou expirado"
                    };
                }

                // Revoga o token antigo
                tokenArmazenado.RevokedAt = DateTime.UtcNow;
                _context.RefreshTokens.Update(tokenArmazenado);

                // Gera novos tokens
                return await GerarRespostaAutenticacao(tokenArmazenado.Usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao renovar token");
                return new AuthResponse
                {
                    Sucesso = false,
                    Mensagem = "Ocorreu um erro ao renovar o token"
                };
            }
        }

        public async Task RevogarRefreshTokenAsync(string usuarioId)
        {
            try
            {
                var tokens = await _context.RefreshTokens
                    .Where(rt => rt.UsuarioId == Guid.Parse(usuarioId) && rt.RevokedAt == null)
                    .ToListAsync();

                foreach (var token in tokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao revogar refresh tokens");
                throw;
            }
        }

        public async Task<UsuarioInfoResponse> ObterUsuarioPorIdAsync(string usuarioId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Paciente)
                .Include(u => u.Medico)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(usuarioId));

            if (usuario == null)
            {
                return null;
            }

            return new UsuarioInfoResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil,
                Telefone = usuario.Telefone,
                CPF = usuario.Paciente?.CPF,
                CRM = usuario.Medico?.CRM,
                DataNascimento = usuario.Paciente?.DataNascimento
            };
        }

        private async Task<AuthResponse> GerarRespostaAutenticacao(Usuario usuario)
        {
            var accessToken = _jwtService.GenerateToken(usuario);
            var refreshToken = GerarRefreshToken();

            await _context.RefreshTokens.AddAsync(new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 dias de validade
                UsuarioId = usuario.Id,
                DeviceInfo = ObterDeviceInfo()
            });

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Sucesso = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiracao = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                Usuario = await ObterUsuarioPorIdAsync(usuario.Id.ToString())
            };
        }

        private string GerarRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "")
                .Replace("+", "")
                .Replace("/", "");
        }

        private string ObterDeviceInfo()
        {
            var userAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();
            return string.IsNullOrEmpty(userAgent) ? "Desconhecido" : userAgent;
        }

    }
}