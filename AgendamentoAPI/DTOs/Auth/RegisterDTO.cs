using System.ComponentModel.DataAnnotations;

namespace AgendamentoAPI.DTOs.Auth
{
    public class RegistroDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [Compare("Senha", ErrorMessage = "Senhas não conferem")]
        [DataType(DataType.Password)]
        public string ConfirmarSenha { get; set; }

        [Required(ErrorMessage = "Perfil é obrigatório")]
        public string Perfil { get; set; } // "Paciente" ou "Medico"

        // Campos específicos de Paciente
        public string CPF { get; set; }
        public DateTime? DataNascimento { get; set; }

        // Campos específicos de Medico
        public string CRM { get; set; }
    }
}