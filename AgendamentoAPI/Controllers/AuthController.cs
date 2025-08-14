using Microsoft.AspNetCore.Mvc;
using AgendamentoAPI.DTOs.Auth;
using AgendamentoAPI.Models;
using AgendamentoAPI.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AgendamentoAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Registra um novo usuário (Paciente ou Médico)
        /// </summary>
        [HttpPost("registro")]
        public async Task<ActionResult<AuthResponse>> Registrar([FromBody] RegistroDto registroDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _authService.RegistrarAsync(registroDto);

                if (!resultado.Sucesso)
                {
                    return BadRequest(new { resultado.Mensagem });
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário");
                return StatusCode(500, "Ocorreu um erro interno ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Autentica um usuário e retorna tokens JWT
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _authService.LoginAsync(loginDto);

                if (!resultado.Sucesso)
                {
                    return Unauthorized(new { resultado.Mensagem });
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao autenticar usuário");
                return StatusCode(500, "Ocorreu um erro interno ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Renova o access token usando um refresh token válido
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _authService.RefreshTokenAsync(request.Token);

                if (!resultado.Sucesso)
                {
                    return Unauthorized(new { resultado.Mensagem });
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao renovar token");
                return StatusCode(500, "Ocorreu um erro interno ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Revoga o refresh token atual do usuário (logout)
        /// </summary>
        [HttpPost("revogar")]
        [Authorize]
        public async Task<IActionResult> Revogar()
        {
            try
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _authService.RevogarRefreshTokenAsync(usuarioId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao revogar token");
                return StatusCode(500, "Ocorreu um erro interno ao processar sua solicitação");
            }
        }

        /// <summary>
        /// Obtém informações do usuário autenticado
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UsuarioInfoResponse>> ObterInformacoesUsuario()
        {
            try
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var usuario = await _authService.ObterUsuarioPorIdAsync(usuarioId);

                if (usuario == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter informações do usuário");
                return StatusCode(500, "Ocorreu um erro interno ao processar sua solicitação");
            }
        }
    }
}