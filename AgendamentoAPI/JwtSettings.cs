using System.ComponentModel.DataAnnotations;

namespace AgendamentoAPI.Models.Configuration
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        [Required(AllowEmptyStrings = false, ErrorMessage = "A chave secreta (Key) do JWT não pode ser vazia.")]
        public string Key { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        public string Issuer { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        public string Audience { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "A expiração do token (ExpirationInMinutes) deve ser de no mínimo 1 minuto.")]
        public int ExpirationInMinutes { get; set; }
    }
}

