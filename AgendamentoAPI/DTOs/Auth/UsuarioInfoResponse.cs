namespace AgendamentoAPI.DTOs.Auth
{
    public class UsuarioInfoResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Perfil { get; set; } 
        public string Telefone { get; set; }
        public string CPF { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string CRM { get; set; }
    }
}