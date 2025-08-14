using System.ComponentModel.DataAnnotations;

namespace AgendamentoAPI.DTOs.Auth
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "O refresh token é obrigatório")]
        public string Token { get; set; }
    }
}