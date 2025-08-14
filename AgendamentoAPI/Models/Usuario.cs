using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendamentoAPI.Models
{
    [Table("Usuario", Schema = "agendamento")]
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, StringLength(100)]
        public string Nome { get; set; }

        [Required, StringLength(100), EmailAddress]
        public string Email { get; set; }

        [StringLength(20)]
        public string Telefone { get; set; }

        [Required]
        public string SenhaHash { get; set; }

        [Required, StringLength(20)]
        public string Perfil { get; set; } 

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }

        public Paciente? Paciente { get; set; }
        public Medico? Medico { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        public ICollection<Consulta> ConsultasComoPaciente { get; set; }
        public ICollection<Consulta> ConsultasComoMedico { get; set; }
    }
}