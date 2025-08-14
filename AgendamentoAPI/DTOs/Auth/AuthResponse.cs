namespace AgendamentoAPI.DTOs.Auth
{
    public class AuthResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? Expiracao { get; set; }
        public UsuarioInfoResponse Usuario { get; set; }
    }
}